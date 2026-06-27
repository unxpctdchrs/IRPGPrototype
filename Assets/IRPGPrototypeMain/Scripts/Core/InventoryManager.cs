using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour 
{
    private Dictionary<ItemData, int> _inventory = new Dictionary<ItemData, int>();

    public void AddItem(ItemData item, int amount)
    {
        if (_inventory.ContainsKey(item))
            _inventory[item] += amount;
        else
            _inventory.Add(item, amount);

        Debug.Log($"[Inventory] Got {amount}x {item.ItemName}. Total owned: {_inventory[item]}");
    }
}