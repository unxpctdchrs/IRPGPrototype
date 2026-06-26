public interface IPartyMember
{
    void SetupPartyMember(CharacterData data, HealthBar linkedUI);
    void PlayAttackAnimation(IBattler target);
}
