using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model.Interfaces;
using MayhemCommon;
using MayhemCommon.MessageObjects;

namespace GameServer.Model.ServerEvents
{
    public class ChatMessagePacket : ServerPacket
    {


        public ChatMessagePacket(ChatItem chatItem) : base(MessageSubCode.Chat, null)
        {
            AddSerializedParameter(chatItem,ClientParameterCode.Object);
        }
    }
}
