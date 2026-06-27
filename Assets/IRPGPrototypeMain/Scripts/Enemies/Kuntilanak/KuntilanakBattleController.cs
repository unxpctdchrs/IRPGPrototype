using System.Collections;
using UnityEngine;

public class KuntilanakBattleController : BaseCharacterBattleController
{
    [SerializeField] private Animator _animator;
    private float _kuntiDamage = 20f;
    private Quaternion _startRotation;

    protected override void Start()
    {
        base.Start();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        _startRotation = transform.rotation;
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
        }
        else 
        {
            _currentController.ReportTurnFinished();
        }
    }

    public override void TakeDamage(float damageAmount)
    {
        if (_healthBar != null) _healthBar.TakeDamage(damageAmount);
        base.TakeDamage(damageAmount);
    }

    public void OnAttackHitTarget()
    {
        if (_currentTarget != null)
        { 
            _currentTarget.TakeDamage(_kuntiDamage);
            PlayHitFeedback();
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
}
