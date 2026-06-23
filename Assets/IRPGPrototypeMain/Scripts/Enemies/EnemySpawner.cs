using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemyToSpawn> _enemiesToSpawn;
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
        if (_enemiesToSpawn == null || _enemiesToSpawn.Count == 0 || _spawnAreas == null || _spawnAreas.Count == 0) 
        {
            Debug.LogWarning("[EnemySpawner] Missing Prefabs or Spawn Areas in the Inspector!");
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
            EnemyToSpawn selectedEnemyProfile = _enemiesToSpawn[Random.Range(0, _enemiesToSpawn.Count)];

            GameObject spawnedObj = Instantiate(selectedEnemyProfile.EnemyModelRepresentation, randomPosition, Quaternion.identity, this.transform);

            if (spawnedObj.TryGetComponent(out EnemyBackpack enemyBackpack))
            {
                enemyBackpack.Initialize(selectedEnemyProfile);
            }
            else
            {
                Debug.LogWarning($"[EnemySpawner] The prefab {selectedEnemyProfile.name} is missing the EnemyBackpack script");
            }
        }
    }
}
