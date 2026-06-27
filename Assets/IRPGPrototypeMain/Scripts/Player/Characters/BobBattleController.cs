using System.Collections;
using UnityEngine;

public class BobBattleController : BasePartyMemberController
{
    [SerializeField] private Animator _bobAnimator;
    private SkillData _currentSkill;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    [SerializeField] private float _dashTime = 0.4f;
    [SerializeField] private AnimationCurve _dashCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    protected override void Start()
    {
        base.Start();
        if (_bobAnimator == null) _bobAnimator = GetComponent<Animator>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    public override void PlayAttackAnimation(IBattler target)
    {
        _currentTarget = target;
        StartCoroutine(AttackDashRoutine(target));
    }

    public override void PlaySkillAnimation(IBattler target, SkillData skill)
    {
        _currentTarget = target;
        _currentSkill = skill;
        
        if (_currentTarget != null) {
            Vector3 targetPos = ((MonoBehaviour)_currentTarget).transform.position;
            targetPos.y = transform.position.y;
            transform.LookAt(targetPos);
            _bobAnimator.SetTrigger("isCastingHeal");
        }
        else 
        {
            _currentController.ReportTurnFinished();
        }
    }

    public void OnSkillHitTarget()
    {
        if (_currentTarget != null && _currentSkill != null)
        { 
            bool targetIsAlly = _currentTarget is IPartyMember;
            if (_currentSkill.Effect == EffectType.Heal)
            {
                if (targetIsAlly)
                {
                    _currentTarget.TakeHealing(_currentSkill.Amount);
                    PlaySkillFeedback(_currentTarget, _currentSkill, false);
                }
                else
                {
                    _currentTarget.TakeDamage(_currentSkill.Amount);
                    PlaySkillFeedback(_currentTarget, _currentSkill, true);
                }
            }
            else if (_currentSkill.Effect == EffectType.Damage)
            {
                _currentTarget.TakeDamage(_currentSkill.Amount);
                PlaySkillFeedback(_currentTarget, _currentSkill, true);
            }
        }
    }

    public void OnSkillAnimationComplete()
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
        _bobAnimator.SetTrigger("isAttacking");
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

    public void OnAttackHitTarget()
    {
        if (_currentTarget != null)
        { 
            _currentTarget.TakeDamage(_attackDamage);
            PlayHitFeedback();
        }
    }

    public void OnAttackAnimationComplete()
    {
        StartCoroutine(ReturnDashRoutine());
    }
}
