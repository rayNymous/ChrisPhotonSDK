using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Quests;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class DialogPagePacket : ServerPacket
    {
        public DialogPagePacket(DialogPageData data) : base(MessageSubCode.DialogPage, null)
        {
            AddSerializedParameter(data, ClientParameterCode.Object);
        }

        public DialogPagePacket(GPlayerInstance instance, DialogPage newPage, String title)
            : base(MessageSubCode.DialogPage)
        {
            IEnumerable<DialogLinkData> links = newPage.GetLinks(instance, true).Where(x => x != null)
                .Select(x => new DialogLinkData {Text = x.Text, Type = x.Type});

            AddSerializedParameter(new DialogPageData
            {
                LeftButtonText = newPage.LeftButtonText,
                LeftButtonEnabled = newPage.IsLeftButtonEnabled,
                Links = links,
                NpcName = title,
                PageId = newPage.Id,
                Text = newPage.Text
            }, ClientParameterCode.Object);
        }
    }
}