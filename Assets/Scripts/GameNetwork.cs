using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

[System.Obsolete]
public class GameNetwork : NetworkDiscovery
{
   public static int PORT = 8887;

        public class StudyMsgType
        {
            public static short PhidgetData = MsgType.Highest + 1;
        };

        public bool RunAsServer;
        private NetworkClient client;

        public void ConfigureAndRun(bool runAsServer, string ip = "")
        {
            Initialize();
            Application.runInBackground = true;
            NetworkTransport.Init();
            RunAsServer = runAsServer;

            if (runAsServer)
            {
                SetupServer(ip);
            }
            else
            {
                SetupClient(ip);
            }
        }

        private void SetupServer(string ip)
        {
            if(ip == "")
            {
                StartAsServer();
            }

            ConnectionConfig connConfig = new ConnectionConfig();
            connConfig.AddChannel(QosType.ReliableSequenced);
            NetworkServer.Configure(connConfig, 1);

            RegisterHandler(MsgType.Error, ErrorHandler);
            RegisterHandler(MsgType.Connect, OnClientConnect);

            NetworkServer.Listen(PORT);
        }

        public void SetupClient(string ip)
        {
            if (ip == "")
            {
                StartAsClient();
            }

            client = new NetworkClient();
            ConnectionConfig connConfig = new ConnectionConfig();
            connConfig.AddChannel(QosType.ReliableSequenced);
            client.Configure(connConfig, 1);

            RegisterHandler(MsgType.Error, ErrorHandler);
            RegisterHandler(MsgType.Connect, OnClientConnect);
            RegisterHandler(MsgType.Disconnect, OnDisconnect);

            if(ip != "")
            {
                client.Connect(ip, PORT);
            }
        }

        public void Send(short msgType, MessageBase msg)
        {
            if (RunAsServer)
            {
                NetworkServer.SendToAll(msgType, msg);
            }
            else
            {
                client.Send(msgType, msg);
            }
        }

        public void RegisterHandler(short msgType, NetworkMessageDelegate msgHandler)
        {
            if (RunAsServer)
            {
                NetworkServer.RegisterHandler(msgType, msgHandler);
            }
            else
            {
                client.RegisterHandler(msgType, msgHandler);
            }
        }

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            if (!RunAsServer)
            {
                string serverAddr = fromAddress.Substring(fromAddress.LastIndexOf(':') + 1);
                client.Connect(serverAddr, PORT);
            }

            StopBroadcast();
        }

        private void OnClientConnect(NetworkMessage netMsg)
        {
            if (!RunAsServer)
            {
                client.Send(MsgType.Ready, new EmptyMessage());
            }
        }

        private void OnDisconnect(NetworkMessage netMsg)
        {
            Debug.Log("Disconnected.");
        }

        private void ErrorHandler(NetworkMessage netMsg)
        {
            ErrorMessage errorMsg = netMsg.ReadMessage<ErrorMessage>();
            NetworkError error = (NetworkError)errorMsg.errorCode;
            Debug.LogError("Network Error: " + error);
        }

        private void OnDestroy()
        {
            if (RunAsServer)
            {
                NetworkServer.Shutdown();
            }
            else
            {
                client.Shutdown();
            }
        }
    }

