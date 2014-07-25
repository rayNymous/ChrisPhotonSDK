using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMO.Framework;

namespace MMO.Photon.Application
{
    class PhotonRequest : IMessage
    {
        private readonly byte _code;
        private readonly Dictionary<byte, object> _parameters;
        private readonly int? _subcode;

        public PhotonRequest(byte code, int? subcode, Dictionary<byte, object> parameters)
        {
            this._code = code;
            this._subcode = subcode;
            this._parameters = parameters;
        }

        public byte Code
        {
            get { return _code; }
        }

        public MessageType Type
        {
            get { return MessageType.Request; }
        }

        public int? SubCode
        {
            get { return _subcode; }
        }

        public Dictionary<byte, object> Parameters
        {
            get { return _parameters; }
        }
    }
}
