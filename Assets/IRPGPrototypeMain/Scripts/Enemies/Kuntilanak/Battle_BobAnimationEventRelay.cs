using UnityEngine;

public class Battle_BobAnimationEventRelay : MonoBehaviour
{
    public BobBattleController MainController;

    public void TriggerHit()
    {
        if (MainController != null)
        {
            MainController.OnSkillHitTarget();
        }
    }

    public void TriggerTurnComplete()
    {
        if (MainController != null)
        {
            MainController.OnSkillAnimationComplete();
        }
    }
}
