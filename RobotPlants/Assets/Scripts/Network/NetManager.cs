using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class NetManager : NetworkManager
{
   
    //When a host starts a server
    public override void OnStartServer()
    {
        Debug.Log("Server Started");

        this.ServerChangeScene("DemoScene");
        
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        GameObject managerClone = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "GameManager"));
        NetworkServer.Spawn(managerClone);


       
    }

    //When a host stops a server
    public override void OnStopServer()
    {
        Debug.Log("Server Stopped");
    }

    //When a client connects to a server
    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Connected to Server");

      //  NetworkServer.Spawn(spawnPrefabs.Find(prefab => prefab.name == "GameManager"));

    }

    //When a client disconnects from a server
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Disconnect from server");
    }

    
}
