using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkCallBacks : NetworkBehaviour 
{


    #region Variables

    //Attribute to be used for variables that have to update from server to client automatically
    [SyncVar(hook = nameof(OnCountChange))]
        int holaCount = 0;


    #endregion

    #region Unity Methods

    //Test var
    public bool sendMessage = false;

    public int HolaCount { get => holaCount; set => holaCount = value; }

    // Update is called once per frame
    void Update()
    {
        //Test method call
        if(isLocalPlayer && sendMessage)
        {
            SendMessageAcrossServer();
            sendMessage = false;
        }        
    }

    #endregion

    #region Network CallBacks

    // When a NetworkBehavior Object has been spawned in the server. 
    public override void OnStartServer()
    {
        Debug.Log("Player has been Spawned in the server");
       
    }

    //When a NetworkBehavior Object has been unspawned from the server. 
    public override void OnStopServer()
    {
        Debug.Log("Server Stopped");

    }

    //Used to Send a message from Client to the Server
    [Command]
    public void SendMessageAcrossServer()
    {
        Debug.Log("Received Hola from Client");
        holaCount++;
        //SendMessageToTargetClient();
        SendMessageToTargetClient();
    }

    //Used to Send a message from the server to all the clients on the server
    [ClientRpc]
    public void SendMessageToAllClients()
    {
        Debug.Log("Too High");
    }

    //Used to send a message to a target client. Add parameter "NetworkConnection conn" if the connection for a particular client is known. 
    //Also can be used in the SendMessageAcrossServer() method to reply back to the client that just sent a message to the server.
    [ClientRpc]
    public void SendMessageToTargetClient()
    {
        Debug.Log("Received Hola from Server");
    }


    //Used to update a syncvar across all clients
    public void OnCountChange(int oldCount, int newCount)
    {
        Debug.Log(" We had " + oldCount + " holas, but now we have " + newCount + " holas");
    }


    #endregion
}
