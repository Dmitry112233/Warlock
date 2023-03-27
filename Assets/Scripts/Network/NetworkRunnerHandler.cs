using Fusion;
using Fusion.Sockets;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;

    public NetworkRunner networkRunner;

    private void Awake()
    {
        NetworkRunner networkRunnerInScene = FindObjectOfType<NetworkRunner>();

        if(networkRunnerInScene != null && !networkRunnerInScene.IsShutdown) 
        {
            networkRunner = networkRunnerInScene;
            Debug.Log("_________________Network runner is assigned and not null");
        }
    }

    void Start()
    {
        Debug.Log("STTTTTTAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAART");

        if(networkRunner == null) 
        {
            Debug.Log("NETWORK RUNNER IS NUUUL");

            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network runner";

            Debug.Log("INSTATIATED NETWORK RUNNER");

            //THIS BLOCK PROBABLY SHOULD BE DELETED
            if (SceneManager.GetActiveScene().name != "MainMenu") 
            {
                Debug.Log("_____________SCENE IS NOT MAIN MENU");
                var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, "TestSession", NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
            }

            Debug.Log($"Server NetworkRunner started");
        }
    }

    public virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized) 
    {

        //var sceneManager = GetComponent<NetworkSceneManagerBase>();
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null) 
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
            Debug.Log("SETED DEFAULT");
        }

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            Initialized = initialized,
            SceneManager = sceneManager,
            PlayerCount = 2
        });

    }

    public void OnJoinLobby() 
    {
        var clientTask = JoinLobby();
    }

    private async Task JoinLobby()
    {
        Debug.Log("JoinLobby started");

        string lobbyID = "OurLobbyID";

        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok) 
        {
            Debug.Log($"Unable to join lobby {lobbyID}");
        }
        else 
        {
            Debug.Log("JoinLobby ok");
        }
    }

    public void CreateGame(string sessionName, string sceneName) 
    {
        Debug.Log($"Create session {sessionName} scene {sceneName} buil Index {SceneUtility.GetBuildIndexByScenePath($"scenes/{sceneName}")}");

        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, NetAddress.Any(), SceneUtility.GetBuildIndexByScenePath($"scenes/{sceneName}"), null);
    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        Debug.Log($"Join session {sessionInfo.Name}");

        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Client, sessionInfo.Name, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
    }
}