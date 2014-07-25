using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;

public class ContainerItem
{
    public String SpriteName { get; set; }
    public UIAtlas Atlas { get; set; }
    public String Name { get; set; }
    public String Description { get; set; }
    public ItemQuality Quality = ItemQuality.Normal;
    public Dictionary<string, float> Stats { get; set; }
    public int ItemLevel { get { return 1; }}
    public MayhemCommon.EquipmentSlot Slot { get; set; }
    public int SlotIndex { get; set; }

    public ContainerItem()
    {
    }

    public ContainerItem(ItemData data)
    {
        SpriteName = data.SpriteName;
        Name = data.Name;
        Description = data.Description;
        Quality = data.Quality;
        Stats = data.Stats;
        Slot = data.Slot;  
    }

    public Color Color
    {
        get
        {
            Color c = Color.white;

            switch (Quality)
            {
                case ItemQuality.Cursed: c = Color.red; break;
                case ItemQuality.Broken: c = new Color(0.4f, 0.2f, 0.2f); break;
                case ItemQuality.Damaged: c = new Color(0.4f, 0.4f, 0.4f); break;
                case ItemQuality.Worn: c = new Color(0.7f, 0.7f, 0.7f); break;
                case ItemQuality.Normal: c = new Color(1.0f, 1.0f, 1.0f); break;
                case ItemQuality.Polished: c = NGUIMath.HexToColor(0xe0ffbeff); break;
                case ItemQuality.Improved: c = NGUIMath.HexToColor(0x93d749ff); break;
                case ItemQuality.Crafted: c = NGUIMath.HexToColor(0x4eff00ff); break;
                case ItemQuality.Superior: c = NGUIMath.HexToColor(0x00baffff); break;
                case ItemQuality.Enchanted: c = NGUIMath.HexToColor(0x7376fdff); break;
                case ItemQuality.Epic: c = NGUIMath.HexToColor(0x9600ffff); break;
                case ItemQuality.Legendary: c = NGUIMath.HexToColor(0xff9000ff); break;
            }
            return c;
        }
    }

}
