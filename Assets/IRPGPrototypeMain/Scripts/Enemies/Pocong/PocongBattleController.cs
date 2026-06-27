using System.Collections;
using UnityEngine;

public class PocongBattleController : BaseCharacterBattleController 
{
    [Header("Pocong Info")]
    [SerializeField] private Animator _pocongBattleAnimator;

    [Header("Loot Drop Settings")]
    [SerializeField] private ItemData _dropItem;
    [SerializeField, Range(0f, 100f)] private float _dropChance = 100f;

    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private float _pocongDamage = 15f;
    private BattlePocongSFX _sfx;

    protected override void Start()
    {
        base.Start();
        if (_pocongBattleAnimator == null) _pocongBattleAnimator = GetComponentInChildren<Animator>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _sfx = GetComponent<BattlePocongSFX>();
    }

    public override void ExecuteTurn(TurnBaseController controller)
    {
        _currentController = controller;
        _currentTarget = controller.GetRandomPlayerTarget();

        if (_currentTarget != null)
        {
            Debug.Log($"Pocong is attacking {((MonoBehaviour)_currentTarget).gameObject.name}");
            StartCoroutine(AttackRoutine(_currentTarget));
            if (_sfx != null) _sfx.PlayDashSFX();
        }
        else
        {
            _currentController.ReportTurnFinished();
        }
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

        PlayHitFeedback();
        if (_sfx != null) _sfx.PlayHitSFX();

        yield return new WaitForSeconds(0.15f);
        StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        Vector3 currentPos = transform.position;
        float dashTime = 0.3f; 
        float jumpHeight = 1.5f; 
        float elapsed = 0f;

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

    protected override void Die()
    {
        if (_dropItem != null && _inventoryManager != null && _battleRewardTracker != null)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= _dropChance)
            {
                _inventoryManager.AddItem(_dropItem, 1);
                _battleRewardTracker.AddDrop(_dropItem);
            }
        }

        base.Die();
    }
}