using UnityEngine;
using TMPro;

public class ResultContentUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _amountText;

    public void Setup(string itemName, int amount)
    {
        _nameText.text = itemName;
        _amountText.text = amount.ToString();
    }
}