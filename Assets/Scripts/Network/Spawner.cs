using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;
    private InputHandler characterInputHandler;

    private SessionListUIHandler sessionListUIHandler;

    private void Awake()
    {
        sessionListUIHandler = FindObjectOfType<SessionListUIHandler>(false);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    {
        if (runner.IsServer) 
        {
            Debug.Log("OnPlayerJoined we are the server. Spawning Player");
            runner.Spawn(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity, player);
        }
        else 
        {
            Debug.Log("OnPlayerJoined"); 
        }

        //Here call the method which check the count of players and respawn them with timer
    }


    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
        if(characterInputHandler == null && NetworkPlayer.Local != null) 
        {
            characterInputHandler = NetworkPlayer.Local.GetComponent<InputHandler>();
        }

        if(characterInputHandler != null) 
        {
            input.Set(characterInputHandler.GetNetworkInput());
        }
    }

    public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("OnConnectedToServer"); }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { Debug.Log("OnConnectFailed"); }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { Debug.Log("OnConnectRequest"); }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    {
        SceneManager.LoadScene(1);
        Debug.Log("________________________________PLAYER LEFT TO MAIN MENU");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    public void OnSceneLoadDone(NetworkRunner runner) { Debug.Log("________________________________SCENE LOOOAD DOOOOONE"); }

    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) 
    {
        if (sessionListUIHandler == null)
            return;

        if(sessionList.Count == 0) 
        {
            Debug.Log("Joined lobby no session found");

            sessionListUIHandler.OnNoSessionFound();
        }
        else 
        {
            sessionListUIHandler.ClearList();

            foreach (SessionInfo sessionInfo in sessionList) 
            {
                sessionListUIHandler.AddToList(sessionInfo);

                Debug.Log($"Found session {sessionInfo.Name} playersCount {sessionInfo.PlayerCount}");
            }
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) 
    {
        SceneManager.LoadScene(1);
        Debug.Log("__________________ON SHUTDOWN");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnDisconnectedFromServer(NetworkRunner runner) { Debug.Log("OnDisconnectedFromServer"); }
}
