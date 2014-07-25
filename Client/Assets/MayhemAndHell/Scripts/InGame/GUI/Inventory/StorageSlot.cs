using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public abstract class StorageSlot : ContainerSlot
{
    public ItemStorage Storage;

    protected override ContainerItem ObservedItem
    {
        get { return Storage != null ? Storage.GetItem(SlotIndex) : null; }
    }

    protected override ContainerItem Replace(ContainerItem item)
    {
        return Storage != null ? Storage.Replace(SlotIndex, item) : item;
    }
}
