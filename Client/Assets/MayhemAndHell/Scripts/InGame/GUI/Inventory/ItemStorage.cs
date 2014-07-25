using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemStorage : MonoBehaviour
{
    public int MaxItemCount = 16;
    public int MaxRows = 4;
    public int MaxColumns = 4;
    public GameObject Template;
    //public UIWidget Background;
    public int Spacing = 128;
    public int Padding = 10;

    List<ContainerItem> _items = new List<ContainerItem>();

    public List<ContainerSlot> Slots = new List<ContainerSlot>(); 
    public List<ContainerItem> Items { get { while (_items.Count < MaxItemCount) _items.Add(null); return _items; } }

    public ContainerItem GetItem(int slot) { return (slot < _items.Count) ? _items[slot] : null; }

    public Gui Gui;

    public void ForceReplace(int slot, ContainerItem item)
    {
        if (slot < MaxItemCount)
        {
            // Removing this prev will stop us from filling list with data
            ContainerItem prev = Items[slot];
            _items[slot] = item;
            Slots[slot].Enable();
        }
    }

    public ContainerItem Replace(int slot, ContainerItem item)
    {
        if (slot < MaxItemCount)
        {
            ContainerItem prev = Items[slot];
            _items[slot] = item;
            return prev;
        }
        return item;
    }

    void Awake()
    {
        Gui = FindObjectOfType<Gui>();

        if (Template != null)
        {
            int count = 0;
            Bounds b = new Bounds();

            for (int y = 0; y < MaxRows; ++y)
            {
                for (int x = 0; x < MaxColumns; ++x)
                {
                    GameObject go = NGUITools.AddChild(gameObject, Template);
                    Transform t = go.transform;
                    t.localPosition = new Vector3(Padding + (x + 0.5f) * Spacing, -Padding - (y + 0.5f) * Spacing, 0f);

                    var slot = go.GetComponent<StorageSlot>();

                    if (slot != null)
                    {
                        Slots.Add(slot);
                        slot.Storage = this;
                        slot.SlotIndex = count;
                    }

                    b.Encapsulate(new Vector3(Padding * 2f + (x + 1) * Spacing, -Padding * 2f - (y + 1) * Spacing, 0f));

                    if (++count >= MaxItemCount)
                    {
                        //if (Background != null)
                        //{
                        //    Background.width = (int)b.size[0] + Padding;
                        //    Background.height = (int)b.size[1] + Padding;
                        //}
                        return;
                    }
                }
            }

            //if (Background != null) Background.transform.localScale = Background.transform.worldToLocalMatrix* b.size;
        }
    }

}

