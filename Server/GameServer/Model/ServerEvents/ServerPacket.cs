using System;
using System.Collections.Generic;
using System.IO;
using ExitGames.Logging;
using GameServer.Model.Interfaces;
using MayhemCommon;
using MMO.Photon.Application;
using ProtoBuf;

namespace GameServer.Model.ServerEvents
{
    public class ServerPacket : PhotonEvent
    {
        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public ServerPacket(MessageSubCode subCode, IPlayer player = null)
            : base((byte) ClientEventCode.ServerPacket, (int?) subCode, new Dictionary<byte, object>())
        {
            Player = player;
            AddParameter(subCode, ClientParameterCode.SubOperationCode);
        }

        public IPlayer Player { get; set; }

        public bool IgnoreSelf
        {
            get { return Player != null; }
        }

        public virtual void LateInitialization(GPlayerInstance player)
        {
        }

        public void AddParameter<T>(T obj, ClientParameterCode code)
        {
            if (Parameters.ContainsKey((byte) code))
            {
                Parameters[(byte) code] = obj;
            }
            else
            {
                Parameters.Add((byte) code, obj);
            }
        }

        public void AddSerializedParameter<T>(T obj, ClientParameterCode code, bool binary = true)
        {
            byte[] b = null;

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                b = new byte[ms.Position];
                byte[] fullB = ms.GetBuffer();
                Array.Copy(fullB, b, b.Length);
            }

            if (Parameters.ContainsKey((byte) code))
            {
                Parameters[(byte) code] = b;
            }
            else
            {
                Parameters.Add((byte) code, b);
            }
        }
    }
}