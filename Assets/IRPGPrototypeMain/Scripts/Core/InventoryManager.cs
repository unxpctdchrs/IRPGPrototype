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

    public bool HasItem(ItemData item, int amount = 1)
    {
        return _inventory.ContainsKey(item) && _inventory[item] >= amount;
    }
 
    public void RemoveItem(ItemData item, int amount = 1)
    {
        if (!HasItem(item, amount)) return;
 
        _inventory[item] -= amount;
 
        if (_inventory[item] <= 0)
            _inventory.Remove(item);
 
        Debug.Log($"[Inventory] Removed {amount}x {item.ItemName}.");
    }
}