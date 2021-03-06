﻿using System;
using MayhemCommon;
using MMO.Framework;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace LoginServer.Operations
{
    public class ListCharacters : Operation
    {
        public ListCharacters(IRpcProtocol protocol, IMessage message)
            : base(protocol, new OperationRequest(message.Code, message.Parameters))
        {
        }

        [DataMember(Code = (byte) ClientParameterCode.UserId, IsOptional = false)]
        public Guid UserId { get; set; }
    }
}