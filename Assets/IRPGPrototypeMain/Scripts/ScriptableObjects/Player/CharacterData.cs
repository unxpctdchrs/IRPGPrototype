using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character Data")]
public class CharacterData : ScriptableObject
{
    public string CharacterName;
    public int MaxHealth;
    public int BaseDamage;
    public GameObject BattlePrefab;
}