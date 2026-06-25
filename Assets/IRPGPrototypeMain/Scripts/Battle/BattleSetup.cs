using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BattleSetup : MonoBehaviour
{
    [Header("Battle Area Slots")]
    [SerializeField] private List<Transform> _enemySlots;
    [SerializeField] private List<Transform> _characterSlots;

    private List<IBattler> _activeEnemies = new List<IBattler>();
    private List<IBattler> _activeParty = new List<IBattler>();
    public List<IBattler> GetActiveEnemies() => _activeEnemies;
    public List<IBattler> GetActiveParty() => _activeParty;

    private ScenePayload _payload;

    [Inject]
    public void Construct(ScenePayload payload)
    {
        _payload = payload;
    }

    private void Start()
    {
        SpawnEnemiesAndPartyFromPayload();
    }

    private void SpawnEnemiesAndPartyFromPayload()
    {
        if (_payload.BattleEnemies == null || _payload.BattleEnemies.Count == 0)
        {
            Debug.LogWarning("[BattleSetupManager] No enemies found in payload");
            return;
        }

        if (_payload.CurrentParty == null || _payload.CurrentParty.Count == 0)
        {
            Debug.LogWarning("[BattleSetupManager] No parties found in payload");
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
                GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnSlot.position, spawnSlot.rotation, spawnSlot);
                if (spawnedEnemy.TryGetComponent(out IBattler battlerComponent))
                {
                    _activeEnemies.Add(battlerComponent);
                }
            }
        }

        for (int i = 0; i < _payload.CurrentParty.Count; i++)
        {
            CharacterData characterData = _payload.CurrentParty[i];
            Transform spawnSlot = _characterSlots[i];

            if (characterData != null && characterData.BattlePrefab != null)
            {
                GameObject spawnedParty = Instantiate(characterData.BattlePrefab, spawnSlot.position, spawnSlot.rotation, spawnSlot);
                
                if (spawnedParty.TryGetComponent(out IBattler battlerComponent))
                {
                    _activeParty.Add(battlerComponent);
                }
            }
        }
    }
}