using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyToSpawn _prefabToSpawn;
    [SerializeField] private int _amountToSpawn = 17;
    [SerializeField] private List<Collider> _spawnAreas;
    [SerializeField] private float _heightOffset = 1.0f;

    void Start()
    {
        SpawnEnemies();
    }

    [ContextMenu("TEST: Spawn Enemy")]
    public void SpawnEnemies()
    {
        if (_prefabToSpawn == null || _spawnAreas == null || _spawnAreas.Count == 0) 
        {
            Debug.LogWarning("[EnemySpawner] Missing Prefab or Spawn Areas in the Inspector!");
            return;
        }

        for (int i = 0; i < _amountToSpawn; i++)
        {
            Collider randomZone = _spawnAreas[Random.Range(0, _spawnAreas.Count)];
            Bounds bounds = randomZone.bounds;

            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);
            float spawnY = bounds.max.y + _heightOffset;

            Vector3 randomPosition = new Vector3(randomX, spawnY, randomZ);
            GameObject spawnedObj = Instantiate(_prefabToSpawn.EnemyModelRepresentation, randomPosition, Quaternion.identity, this.transform);

            if (spawnedObj.TryGetComponent<EnemyBackpack>(out EnemyBackpack enemyBrain))
            {
                enemyBrain.Initialize(_prefabToSpawn);
            }
            else
            {
                Debug.LogWarning($"[EnemySpawner] The prefab {_prefabToSpawn.name} is missing the EnemyBackpack script");
            }
        }
    }
}
