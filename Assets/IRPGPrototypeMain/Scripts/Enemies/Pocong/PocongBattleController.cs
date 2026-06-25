using UnityEngine;

public class PocongBattleController : MonoBehaviour, IBattler
{
    [SerializeField] private Animator _pocongBattleAnimator;
    private BattleController _currentController;

    void Start()
    {
        if (_pocongBattleAnimator == null) _pocongBattleAnimator = GetComponent<Animator>();
    }

    public void ExecuteTurn(BattleController controller)
    {
        _currentController = controller;
        IBattler target = controller.GetRandomPlayerTarget();

        if (target != null)
        {
            Debug.Log($"Pocong chose to attack {((MonoBehaviour)target).gameObject.name}!");
        }

        _pocongBattleAnimator.SetTrigger("isAttacking");
    }

    public void OnAttackAnimationComplete()
    {
        if (_currentController == null) {
            Debug.LogError("[PBC] ERROR: Controller is null! The event fired, but I don't know who to report to.");
            return;
        }
        else
        {
            Debug.Log("Test");    
        }

        _currentController.ReportTurnFinished();
        Debug.Log("[PBC] turn finished, starting next");
    }
}
