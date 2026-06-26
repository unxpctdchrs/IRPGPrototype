using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class OLDKuntilanakBattleController : MonoBehaviour, IBattler
{
    [SerializeField] private Animator _kuntilanakBattleAnimator;
    [SerializeField] private HealthBar _healthBar;

    [Header("Feedback")]
    [SerializeField] private SkinnedMeshRenderer _kuntiMesh;
    [SerializeField] private Color _damageColor = Color.purple;
    private Color _originalColor;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private GameObject _attackVFX;
    [SerializeField] private Vector3 _vfxOffset = new Vector3(0, 1f, 0);

    private TurnBaseController _currentController;
    private IBattler _currentTarget;
    private float _kuntiDamage = 20f;

    void Start()
    {
        if (_kuntilanakBattleAnimator == null) _kuntilanakBattleAnimator = GetComponentInChildren<Animator>();
        if (_kuntiMesh != null) _originalColor = _kuntiMesh.material.color;
    }

    public void ExecuteTurn(TurnBaseController controller)
    {
        _currentController = controller;
        _currentTarget = controller.GetRandomPlayerTarget();

        if (_currentTarget != null)
        {
            Debug.Log($"Mba Kunti is attacking {((MonoBehaviour)_currentTarget).gameObject.name}");
            _kuntilanakBattleAnimator.SetTrigger("isAttacking");
        }
        else
        {
            _currentController.ReportTurnFinished();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (_healthBar != null) _healthBar.TakeDamage(damageAmount);
        StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (_kuntiMesh == null) yield break;
        _kuntiMesh.material.color = _damageColor;
        yield return new WaitForSeconds(0.15f);
        _kuntiMesh.material.color = _originalColor;
    }

    public void OnAttackHitTarget()
    {
        if (_currentTarget != null)
        { 
            _currentTarget.TakeDamage(_kuntiDamage);
            StartCoroutine(HitStopRoutine(0.1f));
            if (_attackVFX != null)
            {
                Vector3 spawnPosition = ((MonoBehaviour)_currentTarget).transform.position + _vfxOffset;
                GameObject vfxInstance = Instantiate(_attackVFX, spawnPosition, _attackVFX.transform.rotation);
                Destroy(vfxInstance, 2.0f);
            }
            if (_impulseSource != null)
            {
                _impulseSource.GenerateImpulse();
            }
        }
    }

    public void OnAttackAnimationComplete()
    {
        if (_currentController != null) _currentController.ReportTurnFinished();
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        Time.timeScale = 0f; 
        yield return new WaitForSecondsRealtime(duration);    
        Time.timeScale = 1f; 
    }
}
