using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MCBattleController : BasePartyMemberController
{
    [SerializeField] private float _dashTime = 1.0f;
    [SerializeField] private AnimationCurve _dashCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private Animator _mcAnimator;
    private Vector3 _startPosition;

    protected override void Start()
    {
        base.Start();
        if (_mcAnimator == null) _mcAnimator = GetComponent<Animator>();
        _startPosition = transform.position;
    }

    public override void PlayAttackAnimation(IBattler target)
    {
        _currentTarget = target;
        StartCoroutine(AttackDashRoutine(target));
    }

    // this function called from the attack animation clip
    public void OnAttackHitTarget()
    {
        if (_currentTarget != null)
        { 
            _currentTarget.TakeDamage(_attackDamage);
            PlayHitFeedback();
        }
    }

    // this is also called from the end of attack animation clip
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

    public override void PlaySkillAnimation(IBattler target, SkillData skill)
    {
        Debug.LogWarning("[MCBattleController] this character has no skills");
        _currentController.ReportTurnFinished();
    }
}
