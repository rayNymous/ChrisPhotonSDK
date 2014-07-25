using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;


public class GlobalStorageSlot : StorageSlot
{
    protected override void OnSwitch(ContainerSlot source)
    {

        if (source as InventorySlot != null)
        {
            // Came from inventory
            //Debug.Log("From Inventory to storage");

            Storage.Gui.GameView.InGameController.SendOperation(new OperationRequest()
            {
                OperationCode = (byte)ClientOperationCode.Game,
                Parameters = new Dictionary<byte, object>()
                {
                    {(byte)ClientParameterCode.SubOperationCode, (byte)MessageSubCode.ItemTransfer},
                    {(byte)ClientParameterCode.Object, new Dictionary<string, object>
                    {
                        {"fromC", (byte)ContainerType.Inventory},
                        {"toC", (byte)ContainerType.GlobalStorage},
                        {"fromS", (byte)source.SlotIndex},
                        {"toS", (byte)SlotIndex}
                    }}
                }
            }, true, 0, false);

        } else if (source as GlobalStorageSlot != null)
        {
            // Came from within storage
            //Debug.Log("Storage to storage");

            Storage.Gui.GameView.InGameController.SendOperation(new OperationRequest()
            {
                OperationCode = (byte)ClientOperationCode.Game,
                Parameters = new Dictionary<byte, object>()
                {
                    {(byte)ClientParameterCode.SubOperationCode, (byte)MessageSubCode.ItemTransfer},
                    {(byte)ClientParameterCode.Object, new Dictionary<string, object>
                    {
                        {"fromC", (byte)ContainerType.GlobalStorage},
                        {"toC", (byte)ContainerType.GlobalStorage},
                        {"fromS", (byte)source.SlotIndex},
                        {"toS", (byte)SlotIndex}
                    }}
                }
            }, true, 0, false);
        } 

    }
}
