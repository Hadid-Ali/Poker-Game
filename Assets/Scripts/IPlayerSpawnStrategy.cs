using UnityEngine;
using Fusion;

public interface IPlayerSpawnStrategy
{
    void SpawnPlayer(NetworkRunner runner, NetworkPrefabRef playerPrefab, PlayerRef player, Transform spawnPoint);
}

public class DefaultSpawnStrategy : IPlayerSpawnStrategy
{
    public void SpawnPlayer(NetworkRunner runner, NetworkPrefabRef playerPrefab, PlayerRef player, Transform spawnPoint)
    {
        Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        runner.Spawn(playerPrefab, position, Quaternion.identity, player);
    }
}