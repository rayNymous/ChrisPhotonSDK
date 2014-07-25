using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;

public class EquipmentSlot : ContainerSlot
{
    public Equipment Equipment;
    public MayhemCommon.EquipmentSlot Slot;

    protected override ContainerItem ObservedItem
    {
        get { return Equipment.GetItem(Slot); }
    }

    protected override ContainerItem Replace(ContainerItem item)
    {
        var oldItem = Equipment.GetItem(Slot);
        Equipment.Items[Slot] = item;

        if (item != null && item.Slot != Slot)
        {
            Equipment.Items[Slot] = oldItem;
            return item;
        }

        return oldItem;
    }

    public override void OnClick()
    {
        Equipment.Gui.GameView.InGameController.SendOperation(new OperationRequest()
        {
            OperationCode = (byte)ClientOperationCode.Game,
            Parameters = new Dictionary<byte, object>()
            {
                {(byte)ClientParameterCode.SubOperationCode, (byte)MessageSubCode.EquipItem},
                {(byte)ClientParameterCode.Object, false},
                {(byte)ClientParameterCode.Object2, (byte)Slot},
            }
        }, true, 0, false);
    }

    public override bool CanBeDroppedHere(ContainerItem item)
    {
        return item.Slot == Slot;
    }

    public override void OnDrag(UnityEngine.Vector2 delta)
    {

    }

    protected override void OnSwitch(ContainerSlot source)
    {
        if (source as InventorySlot != null)
        {
            // Came from inventory
            //Debug.Log("From Inventory to equipment");

            Equipment.Gui.GameView.InGameController.SendOperation(new OperationRequest()
            {
                OperationCode = (byte)ClientOperationCode.Game,
                Parameters = new Dictionary<byte, object>()
                {
                    {(byte)ClientParameterCode.SubOperationCode, (byte)MessageSubCode.ItemTransfer},
                    {(byte)ClientParameterCode.Object, new Dictionary<string, object>
                    {
                        {"fromC", (byte)ContainerType.Inventory},
                        {"toC", (byte)ContainerType.Equipment},
                        {"fromS", (byte)source.SlotIndex},
                        {"toS", (byte)Slot}
                    }}
                }
            }, true, 0, false);

        }
        else if (source as GlobalStorageSlot != null)
        {
            // Came from within storage
            //Debug.Log("Storage to equipment");

            Equipment.Gui.GameView.InGameController.SendOperation(new OperationRequest()
            {
                OperationCode = (byte)ClientOperationCode.Game,
                Parameters = new Dictionary<byte, object>()
                {
                    {(byte)ClientParameterCode.SubOperationCode, (byte)MessageSubCode.ItemTransfer},
                    {(byte)ClientParameterCode.Object, new Dictionary<string, object>
                    {
                        {"fromC", (byte)ContainerType.GlobalStorage},
                        {"toC", (byte)ContainerType.Equipment},
                        {"fromS", (byte)source.SlotIndex},
                        {"toS", (byte)Slot}
                    }}
                }
            }, true, 0, false);
        } 
    }
}
