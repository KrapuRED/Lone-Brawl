using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public DonutEnemySpawner spawner;

    [Header("Spawner Settings")]
    public float timeBetweenSpawn = 0.1f;
    private float spawnTimer = 0f;

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if(spawnTimer <= 0f)
        {
            spawner.SpawnEnemySafely();
            spawnTimer = timeBetweenSpawn;
        }
    }
}
