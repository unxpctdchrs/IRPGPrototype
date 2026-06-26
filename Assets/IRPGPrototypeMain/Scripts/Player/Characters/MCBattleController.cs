using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MCBattleController : BaseCharacterBattleController, IPartyMember
{
    [Header("Character Info")]
    [HideInInspector] public string CharacterName;
    [HideInInspector] public float MaxHP;
    [HideInInspector] public float CurrentHP;
    private HealthBar _healthBar;

    [Space]

    [SerializeField] private float _dashTime = 1.0f;
    [SerializeField] private AnimationCurve _dashCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private BattleUIManager _battleUIManager;
    [SerializeField] private Animator _mcAnimator;

    private Vector3 _startPosition;
    private float _shovelDamage;

    protected override void Start()
    {
        base.Start();
        if (_battleUIManager == null) _battleUIManager = FindFirstObjectByType<BattleUIManager>();
        if (_mcAnimator == null) _mcAnimator = GetComponent<Animator>();
        _startPosition = transform.position;
    }

    public void SetupPartyMember(CharacterData data, HealthBar linkedUI)
    {
        CharacterName = data.CharacterName;
        MaxHP = data.MaxHealth;
        CurrentHP = data.MaxHealth;
        _healthBar = linkedUI;
        _shovelDamage = data.BaseDamage;
    }
    
    public override void ExecuteTurn(TurnBaseController controller)
    {
        _currentController = controller;
        if (_battleUIManager != null) _battleUIManager.OpenActionMenu();
    }

    public override void TakeDamage(float damageAmount)
    {
        if (_healthBar != null) _healthBar.TakeDamage(damageAmount);
        base.TakeDamage(damageAmount);
    }

    public void PlayAttackAnimation(IBattler target)
    {
        _currentTarget = target;
        StartCoroutine(AttackDashRoutine(target));
    }

    public void OnAttackHitTarget()
    {
        if (_currentTarget != null)
        { 
            _currentTarget.TakeDamage(_shovelDamage);
            PlayHitFeedback();
        }
    }

    public void OnAttackAnimationComplete()
    {
        StartCoroutine(ReturnDashRoutine());
    }

    private IEnumerator AttackDashRoutine(IBattler target)
    {
        Vector3 targetPos = ((MonoBehaviour)target).transform.position;
        Vector3 attackPos = targetPos + (_startPosition - targetPos).normalized * 1.5f;
        transform.LookAt(attackPos);

        float elapsed = 0f;
        while (elapsed < _dashTime)
        {
            float time = _dashCurve.Evaluate(elapsed / _dashTime);
            transform.position = Vector3.Lerp(_startPosition, attackPos, time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = attackPos;
        _mcAnimator.SetTrigger("isAttacking");
    }

    private IEnumerator ReturnDashRoutine()
    {
        Vector3 currentPos = transform.position;
        float elapsed = 0f;

        while (elapsed < _dashTime)
        {
            float time = _dashCurve.Evaluate(elapsed / _dashTime);
            transform.position = Vector3.Lerp(currentPos, _startPosition, time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;
        if (_currentController != null) _currentController.ReportTurnFinished();
    }

}
