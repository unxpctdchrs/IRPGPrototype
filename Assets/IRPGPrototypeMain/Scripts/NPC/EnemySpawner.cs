using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyToSpawn _prefabToSpawn;
    [SerializeField] private int _amountToSpawn = 17;
    [SerializeField] private Collider _spawnArea;
    [SerializeField] private float _heightOffset = 0.5f;

    void Start()
    {
        SpawnEnemies();
    }

    [ContextMenu("TEST: Spawn Enemy")]
    public void SpawnEnemies()
    {
        if (_spawnArea == null || _prefabToSpawn == null) return;

        Bounds bounds = _spawnArea.bounds;

        for (int i = 0; i < _amountToSpawn; i++)
        {
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
                Debug.LogWarning($"[EnemySpawner] The prefab {_prefabToSpawn.name} is missing the OverworldEnemy script!");
            }
        }
    }
}
