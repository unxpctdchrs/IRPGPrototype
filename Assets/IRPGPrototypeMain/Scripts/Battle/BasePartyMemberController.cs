using System.Collections.Generic;
using UnityEngine;

public abstract class BasePartyMemberController : BaseCharacterBattleController, IPartyMember
{
    [HideInInspector] public string CharacterName;
    [HideInInspector] public float MaxHP;
    [HideInInspector] public float CurrentHP;

    [SerializeField] protected BattleUIManager _battleUIManager;

    protected float _attackDamage;
    protected List<SkillData> _availableSkills;

    protected override void Start()
    {
        base.Start();
        if (_battleUIManager == null) _battleUIManager = FindFirstObjectByType<BattleUIManager>();
    }

    public void SetupPartyMember(CharacterData data, HealthBar linkedUI)
    {
        CharacterName = data.CharacterName;
        MaxHP = data.MaxHealth;
        CurrentHP = data.MaxHealth;
        _healthBar = linkedUI;
        _attackDamage = data.BaseDamage;
        _availableSkills = data.AvailableSkills;
    }

    public override void ExecuteTurn(TurnBaseController controller)
    {
        _currentController = controller;
        if (_battleUIManager != null) {
            _battleUIManager.UpdateActionMenuSkillButton(_availableSkills);
            _battleUIManager.OpenActionMenu();
        }
    }

    public override void TakeDamage(float damageAmount)
    {
        if (_healthBar != null) _healthBar.TakeDamage(damageAmount);
        base.TakeDamage(damageAmount);
    }

    public abstract void PlayAttackAnimation(IBattler target);

    public abstract void PlaySkillAnimation(IBattler target, SkillData skill);
}