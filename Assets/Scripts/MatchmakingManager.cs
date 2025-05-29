using System;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class MatchmakingManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("UI")]
    [SerializeField] private GameObject waitingPanel;
    
    [Header("Settings")]
    [SerializeField] private string sessionName = "PokerRoom";
    [SerializeField] private int maxPlayers = 2;

    private Vector2 _lastMousePos;
    private NetworkRunner _runner;
    private IPlayerSpawnStrategy _spawnStrategy;
    private bool isGameStarted = false; // Track that match has started

    private async void Start()
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;
        _runner.AddCallbacks(this);

        if (waitingPanel != null)
            waitingPanel.SetActive(true);

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            Scene = sceneInfo,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (!result.Ok)
        {
            Debug.LogError($"Failed to start Fusion runner: {result.ShutdownReason}");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsSharedModeMasterClient) return; // StateAuthority in Shared mode
        if (runner.ActivePlayers.Count() < maxPlayers) return;
        
        var gameplayScene = SceneRef.FromIndex(SceneUtility.
            GetBuildIndexByScenePath("Assets/Scenes/GamePlay.unity"));
        runner.LoadScene(gameplayScene, LoadSceneMode.Single);
        isGameStarted = true; // Track that match has started
    }

    public void OnSceneLoadDone(NetworkRunner runner) { }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsSharedModeMasterClient) return;
        
        var playerObj = runner.GetPlayerObject(player);
        if (playerObj != null)
            runner.Despawn(playerObj);

        if (isGameStarted)
        {
            OnReloadScene(runner);
        }
        else
        {
            if (waitingPanel != null)
                waitingPanel.SetActive(true);
        }
    }

    private void OnReloadScene(NetworkRunner runner)
    {
        // Return to Main Menu Scene
        var mainMenuScene = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/MainMenu.unity"));
        runner.LoadScene(mainMenuScene, LoadSceneMode.Single);
        isGameStarted = false;
    }


    // Fusion Callbacks
    public void OnConnectedToServer(NetworkRunner runner) { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        if(!runner.IsSharedModeMasterClient) return;
        OnReloadScene(runner);
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) => request.Accept();
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    
    
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();
        data.EKeyDown = Input.GetKey(KeyCode.E);
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }

    public void SetSpawnStrategy(IPlayerSpawnStrategy strategy) => _spawnStrategy = strategy;
}
