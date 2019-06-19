using Phidget22;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class LowServer : MonoBehaviour
{
    public int port;
    private Server server;

    private void Awake()
    {
        Debug.Log($"UDP server port: {port}");
        // Create a new UDP echo server
        server = new Server(IPAddress.Any, port);

        // Start the server
        Debug.Log("Server starting...");
        server.Start();
        Debug.Log("Done!");
        
    }

    void Start()
    {
        
    }

    private void Update()
    {
       
    }

    private void RestartServer()
    {
        Debug.Log("Server restarting...");
        server.Restart();
        Debug.Log("Done!");
    }

    private void StopServer()
    {
        Debug.Log("Server closing");
        server.Stop();
        Debug.Log("Done!");
    }

    private void OnApplicationQuit()
    {
        server.ratio.Close();
        StopServer();
    }
}
