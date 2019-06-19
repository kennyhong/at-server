using UnityEngine.Networking.NetworkSystem;

[System.Obsolete]
public class DataSender
{
    private GameNetwork connector;

    public DataSender(GameNetwork networkConnector)
    {
        connector = networkConnector;
    }

    public void SendPhidgetData(float val)
    {
        PhidgetData message = new PhidgetData()
        {
            sensorValue = val
        };

        connector.Send(GameNetwork.StudyMsgType.PhidgetData, message);
    }
}