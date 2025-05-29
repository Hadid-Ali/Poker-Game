using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class PlayerSpawner : SimulationBehaviour
{
    [SerializeField] private string sceneName = "GamePlay";
    [SerializeField] private List<Transform> spawnPoints = new();
    public NetworkPrefabRef playerPrefab;

    private NetworkRunner _runner;
    private bool _hasSpawned = false;

    private void Start()
    {
        _runner = FindObjectOfType<NetworkRunner>();
        StartCoroutine(WaitAndSpawn());
    }

    private IEnumerator WaitAndSpawn()
    {
        while (!SceneManager.GetActiveScene().name.Equals(sceneName))
            yield return null;

        yield return new WaitForSeconds(0.2f); // Optional safety delay

        if (_runner == null || !_runner.IsRunning || _hasSpawned) yield break;
        
        // Get unique spawn point based on player index
        var spawnIndex = _runner.LocalPlayer.RawEncoded % spawnPoints.Count;
        var selectedSpawn = spawnPoints[spawnIndex];

        var player = _runner.Spawn(playerPrefab, selectedSpawn.position, selectedSpawn.rotation, _runner.LocalPlayer);
        _hasSpawned = true;

        player.GetComponent<PlayerController>().PlayerID = _runner.LocalPlayer.PlayerId;
    }
}