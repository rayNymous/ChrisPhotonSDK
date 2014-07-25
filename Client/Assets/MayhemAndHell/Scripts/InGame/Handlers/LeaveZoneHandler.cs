using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using UnityEngine;

namespace Assets.MayhemAndHell.Scripts.InGame.Handlers
{
    public class LeaveZoneHandler : PacketHandler
    {
        public LeaveZoneHandler(InGameController controller) : base(controller)
        {
        }

        public override void OnHandleEvent(EventData eventData)
        {
            Application.LoadLevel("CharacterSelect");
        }

        public override MessageSubCode MessageSubCode
        {
            get { return MessageSubCode.LeaveZone; }
        }
    }
}
