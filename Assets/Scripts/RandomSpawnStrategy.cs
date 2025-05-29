using UnityEngine;
using Fusion;

public class RandomSpawnStrategy : IPlayerSpawnStrategy
{
    private Vector3 center;
    private float radius;

    public RandomSpawnStrategy(Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public void SpawnPlayer(NetworkRunner runner, NetworkPrefabRef playerPrefab, PlayerRef player, Transform _)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 spawnPosition = center + new Vector3(randomCircle.x, 0, randomCircle.y);

        runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
    }
}