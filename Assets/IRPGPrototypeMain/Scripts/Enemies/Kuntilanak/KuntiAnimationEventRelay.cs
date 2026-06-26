using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    [Header("Link to Parent Script")]
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