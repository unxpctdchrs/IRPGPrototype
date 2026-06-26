using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class OLDPocongBattleController : MonoBehaviour, IBattler
{
    [SerializeField] private Animator _pocongBattleAnimator;
    [SerializeField] private HealthBar _healthBar;

    [Header("Feedback")]
    [SerializeField] private MeshRenderer _pocongMesh;
    [SerializeField] private Color _damageColor = Color.red;
    private Color _originalColor;
    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private GameObject _attackVFX;
    [SerializeField] private Vector3 _vfxOffset = new Vector3(0, 1f, 0);

    private TurnBaseController _currentController;
    private IBattler _currentTarget;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private float _pocongDamage = 15f;

    void Start()
    {
        if (_pocongBattleAnimator == null) _pocongBattleAnimator = GetComponentInChildren<Animator>();
        if (_pocongMesh != null) _originalColor = _pocongMesh.material.color;
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    public void ExecuteTurn(TurnBaseController controller)
    {
        _currentController = controller;
        _currentTarget = controller.GetRandomPlayerTarget();

        if (_currentTarget != null)
        {
            Debug.Log($"Pocong is attacking {((MonoBehaviour)_currentTarget).gameObject.name}");
            StartCoroutine(AttackRoutine(_currentTarget));
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
        if (_pocongMesh == null) yield break;
        _pocongMesh.material.color = _damageColor;
        yield return new WaitForSeconds(0.15f);
        _pocongMesh.material.color = _originalColor;
    }

    private IEnumerator AttackRoutine(IBattler target)
    {
        _pocongBattleAnimator.SetTrigger("isAttacking");

        Vector3 targetPos = ((MonoBehaviour)target).transform.position;
        Vector3 attackPos = targetPos + (_startPosition - targetPos).normalized * 1.5f;

        transform.LookAt(attackPos);

        float dashTime = 0.3f; 
        float jumpHeight = 1.5f; 
        float elapsed = 0f;

        while (elapsed < dashTime)
        {
            float timeRatio = elapsed / dashTime;
            Vector3 basePos = Vector3.Lerp(_startPosition, attackPos, timeRatio);
            float arcY = Mathf.Sin(timeRatio * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(basePos.x, basePos.y + arcY, basePos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = attackPos;
        target.TakeDamage(_pocongDamage);

        // feedback
        StartCoroutine(HitStopRoutine(0.1f));
        if (_attackVFX != null)
        {
            Vector3 spawnPosition = ((MonoBehaviour)_currentTarget).transform.position + _vfxOffset;
            GameObject vfxInstance = Instantiate(_attackVFX, spawnPosition, Quaternion.identity);
            Destroy(vfxInstance, 2.0f);
        }
        if (_impulseSource != null)
        {
            _impulseSource.GenerateImpulse();
        }

        yield return new WaitForSeconds(0.15f);
        StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        Vector3 currentPos = transform.position;
        float dashTime = 0.3f; 
        float jumpHeight = 1.5f; 
        float elapsed = 0f;

        // Hop backward
        while (elapsed < dashTime)
        {
            float timeRatio = elapsed / dashTime;
            Vector3 basePos = Vector3.Lerp(currentPos, _startPosition, timeRatio);
            float arcY = Mathf.Sin(timeRatio * Mathf.PI) * jumpHeight;
            transform.position = new Vector3(basePos.x, basePos.y + arcY, basePos.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = _startPosition;
        transform.rotation = _startRotation; 
        
        _currentController.ReportTurnFinished();
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        Time.timeScale = 0f; 
        yield return new WaitForSecondsRealtime(duration);    
        Time.timeScale = 1f; 
    }
}
