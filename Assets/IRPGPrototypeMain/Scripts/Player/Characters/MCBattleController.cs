using UnityEngine;

public class MCBattleController : MonoBehaviour, IBattler
{
    [SerializeField] private BattleUIManager _battleUIManager;
    private BattleController _currentController;

    void Start()
    {
        if (_battleUIManager == null) _battleUIManager = FindFirstObjectByType<BattleUIManager>();
    }

    public void ExecuteTurn(BattleController controller)
    {
        _currentController = controller;
        if (_battleUIManager != null) 
        {
            _battleUIManager.OpenActionMenu();
        }
    }

    public void PlayAttackAnimation()
    {
        // _mcAnimator.SetTrigger("isAttacking");
        Debug.Log("Playing player animator");
        _currentController.ReportTurnFinished();
    }

    public void OnAttackAnimationComplete()
    {
         _currentController.ReportTurnFinished();
    }
}
