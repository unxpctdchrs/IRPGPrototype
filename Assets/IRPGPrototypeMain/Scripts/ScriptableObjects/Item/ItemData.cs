using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Item Data")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public Sprite ItemIcon;
    public string Description;
}