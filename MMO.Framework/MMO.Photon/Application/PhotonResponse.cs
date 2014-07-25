using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMO.Framework;

namespace MMO.Photon.Application
{
    public class PhotonResponse : IMessage
    {
        private readonly byte _code;
        private readonly Dictionary<byte, object> _parameters;
        private readonly int? _subcode;
        private readonly string _debugMessage;
        private readonly short _returnCode;

        public PhotonResponse(byte code, int? subcode, Dictionary<byte, object> parameters, string debugMessage, short returnCode)
        {
            this._code = code;
            this._subcode = subcode;
            this._parameters = parameters;
            this._debugMessage = debugMessage;
            this._returnCode = returnCode;
        }

        public byte Code
        {
            get { return _code; }
        }

        public string DebugMessage
        {
            get { return _debugMessage; }
        }

        public MessageType Type
        {
            get { return MessageType.Response; }
        }

        public short ReturnCode
        {
            get {return _returnCode; }
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
