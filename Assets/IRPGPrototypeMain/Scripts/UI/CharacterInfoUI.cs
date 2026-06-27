using TMPro;
using UnityEngine;

public class CharacterInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private HealthBar _healthBar;

    public void SetupUI(CharacterData data)
    {
        _nameText.text = data.CharacterName;
        _healthBar.SetupHealthBar(data.MaxHealth, data.MaxHealth);
    }

    public HealthBar GetHealthBar() => _healthBar;
}
