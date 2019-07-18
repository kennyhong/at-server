using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NetCoreServer;
using UnityEngine;
using Phidget22;

class Server : UdpServer
{
    // Start is called before the first frame update
    public VoltageRatioInput ratio = null;

    //Phidget
    VoltageRatioInput attachedDevice = null;

    public string phidgetPassword;
    public string phidgetServerName;
    bool isRemote = false;
    bool notAttached = true;

    public Server(IPAddress address, int port) : base(address, port) { }

    protected override void OnStarted()
    {
        ratio = new VoltageRatioInput();
        ratio.Attach += Ratio_Attach;

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
        }
        catch (PhidgetException ex) { Debug.Log(ex.ToString()); }
        // Start receive datagrams
        ReceiveAsync();
    }

    protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
    {
        string incoming = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        if(!incoming.Equals("ping"))
        {
            Debug.Log(incoming);
        }
        // Echo the message back to the sender
        try
        {
            double numVal = ratio.SensorValue;
            string val = numVal.ToString();
            if (numVal > 0.7)
            {
                Debug.Log(val);
            }
            SendAsync(endpoint, val);
        } catch (PhidgetException pffexception)
        {
            Debug.Log("Too much");
        } finally
        {
            SendAsync(endpoint, "9.001");
        }
    }

    protected override void OnSent(EndPoint endpoint, long sent)
    {
        // Continue receive datagrams
        ReceiveAsync();
    }

    protected override void OnError(SocketError error)
    {
        Debug.Log($"Echo UDP server caught an error with code {error}");
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
}
