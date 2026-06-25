using UnityEngine;

public class KuntilanakBattleController : MonoBehaviour, IBattler
{
    [SerializeField] private Animator _kuntilanakBattleAnimator;

    void Start()
    {
        if (_kuntilanakBattleAnimator == null) _kuntilanakBattleAnimator = GetComponent<Animator>();
    }

    public void ExecuteTurn(BattleController controller)
    {
        IBattler target = controller.GetRandomPlayerTarget();

        if (target != null)
        {
            Debug.Log($"Pocong chose to attack {((MonoBehaviour)target).gameObject.name}!");
        }

        _kuntilanakBattleAnimator.SetTrigger("isAttacking");

        controller.EndCurrentTurn(); 
    }

    public void TakeDamage(float damageAmount)
    {
        throw new System.NotImplementedException();
    }
}
