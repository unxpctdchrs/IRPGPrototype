using UnityEngine;

public class BobBattleController : BaseCharacterBattleController, IPartyMember
{
    [Header("Character Info")]
    [HideInInspector] public string CharacterName;
    [HideInInspector] public float MaxHP;
    [HideInInspector] public float CurrentHP;
    private HealthBar _healthBar;

    [SerializeField] private BattleUIManager _battleUIManager;
    [SerializeField] private Animator _bobAnimator;
    private float _boneDamage;

    protected override void Start()
    {
        base.Start();
        if (_battleUIManager == null) _battleUIManager = FindFirstObjectByType<BattleUIManager>();
        if (_bobAnimator == null) _bobAnimator = GetComponent<Animator>();
    }

    public void SetupPartyMember(CharacterData data, HealthBar linkedUI)
    {
        CharacterName = data.CharacterName;
        MaxHP = data.MaxHealth;
        CurrentHP = data.MaxHealth;
        _healthBar = linkedUI;
        _boneDamage = data.BaseDamage;
    }
    public override void ExecuteTurn(TurnBaseController controller)
    {
        throw new System.NotImplementedException();
    }

    public void PlayAttackAnimation(IBattler target)
    {
        throw new System.NotImplementedException();
    }

}
