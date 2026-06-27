using System.Collections;
using UnityEngine;
using Zenject;

public class KuntilanakBattleController : BaseCharacterBattleController
{
    [SerializeField] private Animator _animator;
    private float _kuntiDamage = 20f;
    private Quaternion _startRotation;

    [Header("Loot Drop Settings")]
    [SerializeField] private ItemData _dropItem;
    [SerializeField, Range(0f, 100f)] private float _dropChance = 50f;
    private BattleKuntilanakSFX _sfx;

    protected override void Start()
    {
        base.Start();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        _startRotation = transform.rotation;
        _sfx = GetComponent<BattleKuntilanakSFX>();
    }

    public override void ExecuteTurn(TurnBaseController controller)
    {
        _currentController = controller;
        _currentTarget = controller.GetRandomPlayerTarget();
        
        if (_currentTarget != null) {
            Vector3 targetPos = ((MonoBehaviour)_currentTarget).transform.position;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);

            _animator.SetTrigger("isAttacking");
            if (_sfx != null) _sfx.PlaySpellCastSFX();
        }
        else 
        {
            _currentController.ReportTurnFinished();
        }
    }

    public void OnAttackHitTarget()
    {
        if (_currentTarget != null)
        { 
            _currentTarget.TakeDamage(_kuntiDamage);
            PlayHitFeedback();
            if (_sfx != null) _sfx.PlayExplosionSFX();
        }
    }

    public void OnAttackAnimationComplete()
    {
        StartCoroutine(ResetRotRoutine());
    }

    private IEnumerator ResetRotRoutine()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Quaternion currentRot = transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.rotation = Quaternion.Slerp(currentRot, _startRotation, t);
            
            yield return null;
        }

        transform.rotation = _startRotation;
        if (_currentController != null) _currentController.ReportTurnFinished();
    }

    protected override void Die()
    {
        if (_dropItem != null && _inventoryManager != null)
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
