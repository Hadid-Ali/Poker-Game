using UnityEngine;

public class CustomSpawnerSetup : MonoBehaviour
{
    [SerializeField] private MatchmakingManager matchmakingManager;

    private void Awake()
    {
        // Attach this in Awake or Start
        matchmakingManager.SetSpawnStrategy(new RandomSpawnStrategy(Vector3.zero, 10f));
    }
}