using System;
using MMO.Framework;

namespace SubServerCommon.Data.ClientData
{
    public class UserData : IClientData
    {
        public Guid UserId { get; set; }
        public Guid CharacterId { get; set; }
    }
}