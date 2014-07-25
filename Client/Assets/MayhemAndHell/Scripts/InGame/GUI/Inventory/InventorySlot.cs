
using System.Collections.Generic;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;

public class InventorySlot : StorageSlot
{
    protected override void OnSwitch(ContainerSlot source)
    {
        if (source as InventorySlot != null)
        {
            // Came from within
            //Debug.Log("Inventory to inventory ");

            Storage.Gui.GameView.InGameController.SendOperation(new OperationRequest()
            {
                OperationCode = (byte)ClientOperationCode.Game,
                Parameters = new Dictionary<byte, object>()
                {
                    {(byte)ClientParameterCode.SubOperationCode, (byte)MessageSubCode.ItemTransfer},
                    {(byte)ClientParameterCode.Object, new Dictionary<string, object>
                    {
                        {"fromC", (byte)ContainerType.Inventory},
                        {"toC", (byte)ContainerType.Inventory},
                        {"fromS", (byte)source.SlotIndex},
                        {"toS", (byte)SlotIndex}
                    }}
                }
            }, true, 0, false);

        }
        else if (source as GlobalStorageSlot != null)
        {
            // Came from storage
            //Debug.Log("Global to inventory");
            Storage.Gui.GameView.InGameController.SendOperation(new OperationRequest()
            {
                OperationCode = (byte)ClientOperationCode.Game,
                Parameters = new Dictionary<byte, object>()
                {
                    {(byte)ClientParameterCode.SubOperationCode, (byte)MessageSubCode.ItemTransfer},
                    {(byte)ClientParameterCode.Object, new Dictionary<string, object>
                    {
                        {"fromC", (byte)ContainerType.GlobalStorage},
                        {"toC", (byte)ContainerType.Inventory},
                        {"fromS", (byte)source.SlotIndex},
                        {"toS", (byte)SlotIndex}
                    }}
                }
            }, true, 0, false);
        } 
    }
}
