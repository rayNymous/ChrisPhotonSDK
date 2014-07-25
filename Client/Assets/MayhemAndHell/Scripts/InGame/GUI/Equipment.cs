using System.Collections.Generic;
using System.Linq;
using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;

public class Equipment : MonoBehaviour
{
    public Dictionary<MayhemCommon.EquipmentSlot, ContainerItem> Items;
    public List<EquipmentSlot> Slots; 
    private Dictionary<MayhemCommon.EquipmentSlot, EquipmentSlot> _slots; 

    public Gui Gui;

	// Use this for initialization
	void Start () {
        Gui = FindObjectOfType<Gui>();
        NGUITools.SetActive(gameObject, false);
        Items = new Dictionary<MayhemCommon.EquipmentSlot, ContainerItem>();
        _slots = new Dictionary<MayhemCommon.EquipmentSlot, EquipmentSlot>();

	    var slots = gameObject.GetComponentsInChildren<EquipmentSlot>();

        foreach (var slot in Slots)
	    {
            _slots.Add(slot.Slot, slot);
	    }
	}

    public void UpdateData(ContainerData data)
    {
        int index = 0;
        foreach (var itemData in data.Items)
        {
            if (itemData != null)
            {
                ForceEquip(new ContainerItem(itemData)
                {
                    Atlas = Gui.IconAtlas
                }, itemData.Slot);
            }
            index++;
        }
    }

    public void ForceEquip(ContainerItem item, MayhemCommon.EquipmentSlot slot)
    {
        if (Items.ContainsKey(slot))
        {
            Items[slot] = item;
        }
        else
        {
            Items.Add(slot, item);
        }

        _slots[slot].Enable();
    }

    public ContainerItem GetItem(MayhemCommon.EquipmentSlot slot)
    {
        ContainerItem item;
        Items.TryGetValue(slot, out item);
        return item;
    }

    public void Close()
    {
        NGUITools.SetActive(gameObject, false);
    }

    public void Show()
    {
        NGUITools.SetActive(gameObject, true);
    }
}
