using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public abstract class BaseCharacterBattleController : MonoBehaviour, IBattler
{
    [Header("Base Feedback")]
    [SerializeField] protected Renderer _meshRenderer;
    [SerializeField] protected Color _damageColor = Color.red;
    [SerializeField] protected CinemachineImpulseSource _impulseSource;
    [SerializeField] protected GameObject _attackVFX;
    [SerializeField] protected Vector3 _vfxOffset = new Vector3(0, 1f, 0);

    protected Color _originalColor;
    protected TurnBaseController _currentController;
    protected IBattler _currentTarget;
    protected static bool IsTimeFrozen = false;

    // virtual allows child scripts to add their own Start logic while keeping this
    protected virtual void Start()
    {
        if (_meshRenderer != null) _originalColor = _meshRenderer.material.color;
        if (_impulseSource == null) _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public abstract void ExecuteTurn(TurnBaseController controller);

    public virtual void TakeDamage(float damageAmount)
    {
        StartCoroutine(DamageFlashRoutine());
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

    protected IEnumerator HitStopRoutine(float duration)
    {
        IsTimeFrozen = true;
        Time.timeScale = 0f; 
        
        yield return new WaitForSecondsRealtime(duration);    
        
        Time.timeScale = 1f; 
        IsTimeFrozen = false;
    }
}