using System.Collections.Generic;
using System.Diagnostics;

public class BattleRewardTracker
{
    public List<ItemData> DroppedItems = new List<ItemData>();

    public void AddDrop(ItemData item)
    {
        DroppedItems.Add(item);
    }

    public void ClearRewards()
    {
        DroppedItems.Clear();
    }
}