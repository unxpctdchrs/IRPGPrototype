using UnityEngine;

public class Battle_KuntiAnimationEventRelay : MonoBehaviour
{
    public KuntilanakBattleController MainController;

    public void TriggerHit()
    {
        if (MainController != null)
        {
            MainController.OnAttackHitTarget();
        }
    }

    public void TriggerTurnComplete()
    {
        if (MainController != null)
        {
            MainController.OnAttackAnimationComplete();
        }
    }
}