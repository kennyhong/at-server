using Phidget22;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorClient : MonoBehaviour
{
    public string ip;
    public int port;
    private Client client;
    public VoltageRatioInput ratio = null;

    //Phidget
    VoltageRatioInput attachedDevice = null;

    public string phidgetPassword;
    public string phidgetServerName;
    bool isRemote = false;
    bool notAttached = true;

    // Start is called before the first frame update
    void Start()
    {

        Debug.Log($"UDP server address: {ip}");
        Debug.Log($"UDP server port: {port}");

        // Create a new TCP chat client
        client = new Client(ip, port);

        ratio = new VoltageRatioInput();
        ratio.Attach += Ratio_Attach;
        // Connect the client
        Debug.Log("Client connecting...");
        client.Connect();
        Debug.Log("Done!");
        try
        { //set all the values grabbed from command line.  these values have defaults that are set in ExampleUtils.cs, you can check there to see them.
            ratio.Channel = 0; //selects the channel on the device to open
            ratio.DeviceSerialNumber = 538053; //selects the device or hub to open
            ratio.HubPort = 0; //selects the port on the hub to open
            ratio.IsHubPortDevice = true; //is the device a port on a VINT hub?

            if (isRemote) //are we trying to open a remote device?
            {
                ratio.IsRemote = true;
                Net.EnableServerDiscovery(ServerType.Device); //turn on network scan
                if (phidgetPassword != null && phidgetServerName != null)
                    Net.SetServerPassword(phidgetServerName, phidgetPassword); //set the password if there is one
            } 
            else
                ratio.IsLocal = true;

            ratio.Open(); //open the device specified by the above parameters
        } catch (PhidgetException ex) { Debug.Log(ex.ToString()); }
    }

    void Ratio_Attach(object sender, Phidget22.Events.AttachEventArgs e)
    {
        attachedDevice = (VoltageRatioInput)sender;

        try
        {
            attachedDevice.SensorType = VoltageRatioSensorType.PN_1139;
        }
        catch (PhidgetException ex) { Debug.Log(ex.ToString()); }

    }

    private void DisconnectClient()
    {
        Debug.Log("Client disconnecting...");
        client.Disconnect();
        Debug.Log("Done!");
    }

    private void StopClient()
    {
        Debug.Log("Client disconnecting...");
        client.DisconnectAndStop();
        Debug.Log("Done!");
    }

    // Update is called once per frame
    void Update()
    {
        double sensorVal = 0;
        sensorVal = ratio.SensorValue;
        if(sensorVal > 0.700)
        {
            Debug.Log(ratio.SensorValue);
        }
        client.Send(sensorVal.ToString());
    }

    private void OnApplicationQuit()
    {
        ratio.Close();
        StopClient();
    }
}
