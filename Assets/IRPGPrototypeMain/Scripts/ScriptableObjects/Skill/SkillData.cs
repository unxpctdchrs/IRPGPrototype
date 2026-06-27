using UnityEngine;

public enum TargetType { Enemy, Ally, Any }
public enum EffectType { Damage, Heal }

[CreateAssetMenu(fileName = "New Skill", menuName = "Character/Skill")]
public class SkillData : ScriptableObject
{
    public string SkillName;
    public float Amount;
    public TargetType TargetRequirement;
    public EffectType Effect;
    public GameObject SkillVFX;
}