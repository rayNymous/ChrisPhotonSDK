using ExitGames.Logging;
using MMO.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMO.Photon.Client
{
    public class PhotonClientHandlerList
    {
        protected readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<int, PhotonClientHandler> _requestHandlerList;
        private readonly Dictionary<int, PhotonClientHandler> _requestHandlerSubList;


        public PhotonClientHandlerList(IEnumerable<PhotonClientHandler> handlers) {
            _requestHandlerList = new Dictionary<int, PhotonClientHandler>();
            _requestHandlerSubList = new Dictionary<int, PhotonClientHandler>();

            foreach (var handler in handlers)
            {
                if (!RegisterHandler(handler))
                {
                    Log.WarnFormat("Attempted to register handler {0} for type {1}:{2}", handler.GetType().Name, handler.Code);
                }
            }
        }

        public bool RegisterHandler(PhotonClientHandler handler)
        {
            var registered = false;

            if ((handler.Type & MessageType.Request) == MessageType.Request)
            {
                if (handler.SubCode.HasValue && !_requestHandlerSubList.ContainsKey(handler.SubCode.Value))
                {
                    Log.Debug("Registered " + handler.GetType().Name + "By SubCode");
                    _requestHandlerSubList.Add(handler.SubCode.Value, handler);
                    registered = true;
                }
                else if (!_requestHandlerList.ContainsKey(handler.Code))
                {
                    Log.Debug("Registered " + handler.GetType().Name + "By Code");
                    _requestHandlerList.Add(handler.Code, handler);
                    registered = true;
                }
                else
                {
                    Log.ErrorFormat("RequestHandler list already contains handler for {0} - cannot add {1}", handler.Code, handler.GetType().Name);
                }
            }
            return registered;
        }

        public bool HandleMessage(IMessage message, PhotonClientPeer peer)
        {
            bool handled = false;
            Log.Debug("ClientHandler Received message " + message.Code);
            switch (message.Type)
            {
                case MessageType.Request:
                    if (message.SubCode.HasValue && _requestHandlerSubList.ContainsKey(message.SubCode.Value))
                    {
                        Log.Debug("Accessed " + _requestHandlerSubList[message.SubCode.Value].GetType().Name +"By SubCode");
                        _requestHandlerSubList[message.SubCode.Value].HandleMessage(message, peer);
                        handled = true;
                    }
                    else 
                    if (_requestHandlerList.ContainsKey(message.Code))
                    {
                        Log.Debug("Accessed " + _requestHandlerList[message.Code].GetType().Name + "By Code");
                        _requestHandlerList[message.Code].HandleMessage(message, peer);
                        handled = true;
                    }
                    else
                    {
                        Log.DebugFormat("PhotonClientHandlerList Message handler not found");
                    }
                    break;
            }

            return handled;
        }
    }
}
