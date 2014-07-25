using System.Collections.Generic;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;
using System.Collections;
using EquipmentSlot = MayhemCommon.EquipmentSlot;

public class TestButton : MonoBehaviour
{
    public UIAtlas IconAtlas;
    public string SpriteName;
    public EventNotifications Notifications;
    public ItemStorage Storage;

    public Inventory Inventory;

    public CoinsBar CoinsBar;
    public int CoinsUpdate;

    public DialogWindow DialowWindow;

    void Start()
    {
        var item = new ContainerItem()
        {
            Atlas = IconAtlas,
            Description = "This is a description",
            Name = "Frozen sword",
            Quality = ItemQuality.Legendary,
            SpriteName = SpriteName,
            Stats = new Dictionary<string, float>()
            {
                {"Strength", 5},
                {"Constitution", 8},
                {"Death", -1}
            },
            Slot = MayhemCommon.EquipmentSlot.None
        };

        var itemB = new ContainerItem()
        {
            Atlas = IconAtlas,
            Description = "This is a description",
            Name = "SHIELD",
            Quality = ItemQuality.Superior,
            SpriteName = "shield",
            Stats = new Dictionary<string, float>()
            {
                {"Strength", 5},
                {"Constitution", 8},
                {"Death", -1}
            },
            Slot = MayhemCommon.EquipmentSlot.Head
        };

        Storage.Replace(1, item);
        Storage.Replace(0, itemB);
    }

    private bool _testas = true;

    public void StartTesting()
    {
        Notifications.AddNotification("Testassasd sd");


        //_testas = !_testas;

        //CoinsBar.UpdateCount(100);

        //var items = new List<ContainerItem>();
        //var item = new ContainerItem()
        //{
        //    Atlas = IconAtlas,
        //    Description = "This is a description",
        //    Name = "Frozen sword",
        //    Quality = ItemQuality.Legendary,
        //    SpriteName = SpriteName,
        //    Stats = new Dictionary<string, float>()
        //    {
        //        {"Strength", 5},
        //        {"Constitution", 8},
        //        {"Death", -1}
        //    }
        //};

        //items.Add(item);
        //items.Add(item);
        //items.Add(item);
        //items.Add(item);
        //Storage.Show(items);

        
    }
}
