public interface IBattler
{
    void ExecuteTurn(TurnBaseController controller);
    void TakeDamage(float damageAmount);
    void TakeHealing(float healAmount);
}
