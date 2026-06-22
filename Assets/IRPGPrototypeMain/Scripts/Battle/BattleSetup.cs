using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BattleSetup : MonoBehaviour
{
    [Header("Battle Area Slots")]
    [SerializeField] private List<Transform> _enemySlots;

    // [SerializeField] private List<Transform> _characterSlots;

    private ScenePayload _payload;

    [Inject]
    public void Construct(ScenePayload payload)
    {
        _payload = payload;
    }

    private void Start()
    {
        SpawnEnemiesFromPayload();
    }

    private void SpawnEnemiesFromPayload()
    {
        if (_payload.BattleEnemies == null || _payload.BattleEnemies.Count == 0)
        {
            Debug.LogWarning("[BattleSetupManager] No enemies found in payload");
            return;
        }

        for (int i = 0; i < _payload.BattleEnemies.Count; i++)
        {
            if (i >= _enemySlots.Count)
            {
                Debug.LogWarning($"[BattleSetupManager] Too many enemies, Only {_enemySlots.Count} slots available. Skipping the rest.");
                break;
            }

            GameObject enemyPrefab = _payload.BattleEnemies[i];
            Transform spawnSlot = _enemySlots[i];

            if (enemyPrefab != null)
            {
                Instantiate(enemyPrefab, spawnSlot.position, spawnSlot.rotation, spawnSlot);
                Debug.Log($"[BattleSetupManager] Spawned {enemyPrefab.name} at {spawnSlot.name}");
            }
        }
    }
}