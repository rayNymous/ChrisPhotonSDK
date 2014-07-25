using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LootSlot : ContainerSlot
{
    public LootContainer Loot;
    public int Slot = 0;

    protected override ContainerItem ObservedItem
    {
        get { return Loot != null ? Loot.GetItem(Slot) : null; }
    }

    protected override ContainerItem Replace(ContainerItem item)
    {
        return item;
    }

    protected override void OnSwitch(ContainerSlot source)
    {
    }
}
