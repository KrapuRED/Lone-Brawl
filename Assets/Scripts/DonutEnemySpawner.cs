using System.Security;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class DonutEnemySpawner : MonoBehaviour
{
    public Transform playerTransform;

    [Header("Spawn Rules")]
    public float minSpawnRadius;
    public float maxSpawnRadius;

    public float maxNavMeshSearchDistance;

    

    public void SpawnEnemySafely()
    {
        // generate spawn point in a donut shape, but still raw (not considering navmesh surface just yet)
        Vector2 randomDir2D = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);
        Vector3 rawSpawnPoint = playerTransform.position + new Vector3(randomDir2D.x, 0f, randomDir2D.y) * randomDistance;

        // checking the rawSpawnPoint
        NavMeshHit hit;
        if(NavMesh.SamplePosition(rawSpawnPoint, out hit, maxNavMeshSearchDistance, NavMesh.AllAreas))
        {
            GameObject enemy = EnemyPool.Instance.GetObject();
            enemy.transform.position = hit.position;
            enemy.transform.rotation = Quaternion.identity;

            enemy.SetActive(true);

        }
        else
        {
            Debug.Log("Random Spawn Point rejected!");
        }

    }
}
