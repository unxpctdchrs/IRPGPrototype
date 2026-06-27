using System.Collections.Generic;

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