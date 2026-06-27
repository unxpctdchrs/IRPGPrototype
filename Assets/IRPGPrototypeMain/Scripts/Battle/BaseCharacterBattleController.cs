using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public abstract class BaseCharacterBattleController : MonoBehaviour, IBattler
{
    [Header("Base Feedback")]
    [SerializeField] protected Renderer _meshRenderer;
    [SerializeField] protected Color _damageColor = Color.red;
    [SerializeField] protected CinemachineImpulseSource _impulseSource;
    [SerializeField] protected GameObject _attackVFX;
    [SerializeField] protected Vector3 _vfxOffset = new Vector3(0, 1f, 0);

    [Header("Health Stats")]
    [SerializeField] protected float _maxHP = 100f;
    protected float _currentHP;
    [SerializeField] protected HealthBar _healthBar;

    [Header("Death Feedback")]
    [SerializeField] private GameObject _deathVFX; 
    [SerializeField] private AudioResource _deathSFX;
    [SerializeField] private float _vfxLifetime = 2.0f;

    protected InventoryManager _inventoryManager;
    protected BattleRewardTracker _battleRewardTracker;
    private AudioManager _audioManager;

    [Inject]
    public void Construct(InventoryManager inventoryManager, BattleRewardTracker battleRewardTracker, AudioManager audioManager)
    {
        _inventoryManager = inventoryManager;
        _battleRewardTracker = battleRewardTracker;
        _audioManager = audioManager;
    }

    protected Color _originalColor;
    protected TurnBaseController _currentController;
    protected IBattler _currentTarget;
    protected static bool IsTimeFrozen = false;
    public bool IsDead { get; protected set; } = false;

    // virtual allows child scripts to add their own Start logic while keeping this
    protected virtual void Start()
    {
        _currentHP = _maxHP;

        if (_healthBar != null)
        {
            _healthBar.SetupHealthBar(_currentHP, _maxHP);
        }

        if (_meshRenderer != null) _originalColor = _meshRenderer.material.color;
        if (_impulseSource == null) _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public abstract void ExecuteTurn(TurnBaseController controller);

    public virtual void TakeDamage(float damageAmount)
    {
        if (IsDead) return;

        _currentHP -= damageAmount;
        if (_healthBar != null) _healthBar.UpdateHealth(_currentHP);
        StartCoroutine(DamageFlashRoutine());

        if (_currentHP <= 0)
        {
            _currentHP = 0;
            Die();
        }
    }

    protected IEnumerator DamageFlashRoutine()
    {
        if (_meshRenderer == null) yield break;
        _meshRenderer.material.color = _damageColor;
        yield return new WaitForSeconds(0.15f);
        _meshRenderer.material.color = _originalColor;
    }

    protected void PlayHitFeedback()
    {
        if (!IsTimeFrozen) StartCoroutine(HitStopRoutine(0.1f));

        if (_attackVFX != null && _currentTarget != null)
        {
            Vector3 spawnPosition = ((MonoBehaviour)_currentTarget).transform.position + _vfxOffset;
            GameObject vfxInstance = Instantiate(_attackVFX, spawnPosition, _attackVFX.transform.rotation);
            Destroy(vfxInstance, 2.0f);
        }

        if (_impulseSource != null) _impulseSource.GenerateImpulse();
    }

    protected void PlaySkillFeedback(IBattler target, SkillData skill, bool causedDamage)
    {
        if (causedDamage)
        {
            if (!IsTimeFrozen) StartCoroutine(HitStopRoutine(0.1f));
            if (_impulseSource != null) _impulseSource.GenerateImpulse();
        }

        if (skill.SkillVFX != null)
        {
            Vector3 spawnPosition = ((MonoBehaviour)target).transform.position + _vfxOffset;
            GameObject vfxInstance = Instantiate(skill.SkillVFX, spawnPosition, skill.SkillVFX.transform.rotation);
            Destroy(vfxInstance, 2.0f);
        }
    }

    protected IEnumerator HitStopRoutine(float duration)
    {
        IsTimeFrozen = true;
        Time.timeScale = 0f; 
        
        yield return new WaitForSecondsRealtime(duration);    
        
        Time.timeScale = 1f; 
        IsTimeFrozen = false;
    }

    public void TakeHealing(float healAmount)
    {
        if (IsDead) return;
        _currentHP = Mathf.Min(_currentHP + healAmount, _maxHP);
        if (_healthBar != null) _healthBar.UpdateHealth(_currentHP);
    }

    protected virtual void Die()
    {
        IsDead = true;
        Debug.Log($"[Death] {gameObject.name} has died");

        if (_deathVFX != null)
        {
            GameObject vfx = Instantiate(_deathVFX, transform.position, _deathVFX.transform.rotation);
            Destroy(vfx, _vfxLifetime);
        }

        if (_deathSFX != null)
        {
            _audioManager.PlaySFX(_deathSFX);
        }
        gameObject.SetActive(false); 
    }
}