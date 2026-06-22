using UnityEngine;

public class EnemyBackpack : MonoBehaviour
{
    public EnemyToSpawn EncounterProfile { get; private set; }

    public void Initialize(EnemyToSpawn profile)
    {
        EncounterProfile = profile;
    }
}