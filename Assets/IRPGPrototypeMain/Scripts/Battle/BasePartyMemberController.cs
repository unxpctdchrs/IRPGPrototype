using System.Collections.Generic;
using UnityEngine;

public abstract class BasePartyMemberController : BaseCharacterBattleController, IPartyMember
{
    [HideInInspector] public string CharacterName;
    [HideInInspector] public float MaxHP;
    public float CurrentHP => _currentHP;

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
        _currentHP = data.MaxHealth;
        _healthBar = linkedUI;
        _attackDamage = data.BaseDamage;
        _availableSkills = data.AvailableSkills;

        if (_healthBar != null) 
        {
            _healthBar.SetupHealthBar(_currentHP, MaxHP);
        }
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
        base.TakeDamage(damageAmount);
    }

    public abstract void PlayAttackAnimation(IBattler target);

    public abstract void PlaySkillAnimation(IBattler target, SkillData skill);
}