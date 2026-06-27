using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class OLDMCBattleController : MonoBehaviour, IBattler, IPartyMember
{
    [Header("Info")]
    [HideInInspector] public string CharacterName;
    [HideInInspector] public float MaxHP;
    [HideInInspector] public float CurrentHP;
    private HealthBar _myDynamicHealthBar;

    [Space]
    [SerializeField] private float _dashTime = 1.0f;
    [SerializeField] private AnimationCurve _dashCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private BattleUIManager _battleUIManager;
    [SerializeField] private Animator _mcAnimator;

    [Header("Feedback")]
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private GameObject _swordSlashVfxPrefab;
    [SerializeField] private Vector3 _vfxOffset = new Vector3(0, 1f, 0);
    [SerializeField] private SkinnedMeshRenderer _mcMesh;
    [SerializeField] private Color _damageColor = Color.red;
    private Color _originalColor;

    private TurnBaseController _currentController;
    private IBattler _currentTarget;
    private Vector3 _startPosition;
    private float _shovelDamage = 14.0f;

    void Start()
    {
        if (_battleUIManager == null) _battleUIManager = FindFirstObjectByType<BattleUIManager>();
        if (_mcAnimator == null) _mcAnimator = GetComponent<Animator>();
        _startPosition = transform.position;
        if (_impulseSource == null) _impulseSource = GetComponent<CinemachineImpulseSource>();
        if (_mcMesh != null) _originalColor = _mcMesh.material.color;
    }

    public void ExecuteTurn(TurnBaseController controller)
    {
        _currentController = controller;
        if (_battleUIManager != null) 
        {
            // _battleUIManager.OpenActionMenu();
        }
    }

    public void PlayAttackAnimation(IBattler target)
    {
        _currentTarget = target;
        StartCoroutine(AttackDashRoutine(target));
    }

    public void OnAttackAnimationComplete()
    {
        StartCoroutine(ReturnDashRoutine());
    }

    public void TakeDamage(float damageAmount)
    {
        if (_myDynamicHealthBar != null) _myDynamicHealthBar.TakeDamage(damageAmount);
        StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (_mcMesh == null) yield break;
        _mcMesh.material.color = _damageColor;
        yield return new WaitForSeconds(0.15f);
        _mcMesh.material.color = _originalColor;
    }

    public void OnAttackHitTarget()
    {
        if (_currentTarget != null)
        { 
            _currentTarget.TakeDamage(_shovelDamage);
            StartCoroutine(HitStopRoutine(0.1f));
            if (_swordSlashVfxPrefab != null)
            {
                Vector3 spawnPosition = ((MonoBehaviour)_currentTarget).transform.position + _vfxOffset;
                GameObject vfxInstance = Instantiate(_swordSlashVfxPrefab, spawnPosition, Quaternion.identity);
                Destroy(vfxInstance, 2.0f);
            }
            if (_impulseSource != null)
            {
                _impulseSource.GenerateImpulse();
            }
        }
    }

    private IEnumerator AttackDashRoutine(IBattler target)
    {
        Vector3 targetPos = ((MonoBehaviour)target).transform.position;
        Vector3 attackPos = targetPos + (_startPosition - targetPos).normalized * 1.5f;
        transform.LookAt(attackPos);

        float elapsed = 0f;

        while (elapsed < _dashTime)
        {
            float linearTime = elapsed / _dashTime;
            float curvedTime = _dashCurve.Evaluate(linearTime);
            transform.position = Vector3.Lerp(_startPosition, attackPos, curvedTime);
            
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
            float linearTime = elapsed / _dashTime;
            float curvedTime = _dashCurve.Evaluate(linearTime);
            transform.position = Vector3.Lerp(currentPos, _startPosition, curvedTime);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _startPosition;
        transform.rotation = Quaternion.identity;

        if (_currentController != null)
        {
            _currentController.ReportTurnFinished();
        }
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        Time.timeScale = 0f; 
        yield return new WaitForSecondsRealtime(duration);    
        Time.timeScale = 1f; 
    }

    public void SetupPartyMember(CharacterData data, HealthBar linkedUI)
    {
        CharacterName = data.CharacterName;
        MaxHP = data.MaxHealth;
        CurrentHP = data.MaxHealth;
        _myDynamicHealthBar = linkedUI;
    }

    public void TakeHealing(float healAmount)
    {
        throw new System.NotImplementedException();
    }

    public void ExecuteSkill(SkillData skill, IBattler target)
    {
        throw new System.NotImplementedException();
    }

    public void PlaySkillAnimation(IBattler target, SkillData skill)
    {
        throw new System.NotImplementedException();
    }
}
