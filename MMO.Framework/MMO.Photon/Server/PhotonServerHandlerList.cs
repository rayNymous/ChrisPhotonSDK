using ExitGames.Logging;
using MMO.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMO.Photon.Server
{
    public class PhotonServerHandlerList
    {
        private readonly DefaultRequestHandler _defaultRequestHandler;
        private readonly DefaultResponseHandler _defaultResponseHandler;
        private readonly DefaultEventHandler _defaultEventHander;

        protected readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<int, PhotonServerHandler> _requestHandlerList;
        private readonly Dictionary<int, PhotonServerHandler> _responseHandlerList;
        private readonly Dictionary<int, PhotonServerHandler> _eventHandlerList;
        private readonly Dictionary<int, PhotonServerHandler> _requestHandlerSubList;
        private readonly Dictionary<int, PhotonServerHandler> _responseHandlerSubList;
        private readonly Dictionary<int, PhotonServerHandler> _eventHandlerSubList;

        public PhotonServerHandlerList(IEnumerable<PhotonServerHandler> handlers, DefaultRequestHandler defaultRequestHandler, DefaultResponseHandler defaultResponseHandler, DefaultEventHandler defaultEventHandler) 
        {
            this._defaultRequestHandler = defaultRequestHandler;
            this._defaultResponseHandler = defaultResponseHandler;
            this._defaultEventHander = defaultEventHandler;

            _requestHandlerList = new Dictionary<int, PhotonServerHandler>();
            _responseHandlerList = new Dictionary<int, PhotonServerHandler>();
            _eventHandlerList = new Dictionary<int, PhotonServerHandler>();

            _requestHandlerSubList = new Dictionary<int, PhotonServerHandler>();
            _responseHandlerSubList = new Dictionary<int, PhotonServerHandler>();
            _eventHandlerSubList = new Dictionary<int, PhotonServerHandler>();

            foreach (PhotonServerHandler handler in handlers)
            {
                if (!RegisterHandler(handler))
                {
                    Log.WarnFormat("Attempted to register handler {0} for type {1}:{2}", handler.GetType().Name, handler.Type, handler.Code);
                }
            }
        }

        public bool RegisterHandler(PhotonServerHandler handler)
        {
            var registered = false;

            if ((handler.Type & MessageType.Request) == MessageType.Request)
            {
                if (handler.SubCode.HasValue && !_requestHandlerSubList.ContainsKey(handler.SubCode.Value))
                {
                    _requestHandlerSubList.Add(handler.SubCode.Value, handler);
                    registered = true;
                }
                else if (!_requestHandlerList.ContainsKey(handler.Code))
                {
                    _requestHandlerList.Add(handler.Code, handler);
                    registered = true;
                }
                else
                {
                    Log.ErrorFormat("RequestHandler list already contains handler for {0} - cannot add {1}", handler.Code, handler.GetType().Name);
                }
            }
            if ((handler.Type & MessageType.Response) == MessageType.Response)
            {
                if (handler.SubCode.HasValue && !_responseHandlerSubList.ContainsKey(handler.SubCode.Value))
                {
                    _responseHandlerSubList.Add(handler.SubCode.Value, handler);
                    registered = true;
                }
                else if (!_responseHandlerList.ContainsKey(handler.Code))
                {
                    _responseHandlerList.Add(handler.Code, handler);
                    registered = true;
                }
                else
                {
                    Log.ErrorFormat("ResponseHandler list already contains handler for {0} - cannot add {1}", handler.Code, handler.GetType().Name);
                }
            }
            if ((handler.Type & MessageType.Async) == MessageType.Async)
            {
                if (handler.SubCode.HasValue && !_eventHandlerSubList.ContainsKey(handler.SubCode.Value))
                {
                    _eventHandlerSubList.Add(handler.SubCode.Value, handler);
                    registered = true;
                }
                else if (!_eventHandlerList.ContainsKey(handler.Code))
                {
                    _eventHandlerList.Add(handler.Code, handler);
                    registered = true;
                }
                else
                {
                    Log.ErrorFormat("EventHandler list already contains handler for {0} - cannot add {1}", handler.Code, handler.GetType().Name);
                }
            }


            return registered;
        }

        public bool HandleMessage(IMessage message, PhotonServerPeer peer)
        {
            bool handled = false;
            switch (message.Type)
            {
                case MessageType.Request:
                    if (message.SubCode.HasValue && _requestHandlerSubList.ContainsKey(message.SubCode.Value))
                    {
                        _requestHandlerSubList[message.SubCode.Value].HandleMessage(message, peer);
                        handled = true;
                    }
                    else if (!message.SubCode.HasValue && _requestHandlerList.ContainsKey(message.Code))
                    {
                        _requestHandlerList[message.Code].HandleMessage(message, peer);
                        handled = true;
                    }
                    else
                    {
                        _defaultRequestHandler.HandleMessage(message, peer);
                    }
                    break;
                case MessageType.Response:
                    if (message.SubCode.HasValue && _responseHandlerSubList.ContainsKey(message.SubCode.Value))
                    {
                        _responseHandlerSubList[message.SubCode.Value].HandleMessage(message, peer);
                        handled = true;
                    }
                    else if (!message.SubCode.HasValue && _responseHandlerList.ContainsKey(message.Code))
                    {
                        _responseHandlerList[message.Code].HandleMessage(message, peer);
                        handled = true;
                    }
                    else
                    {
                        _defaultResponseHandler.HandleMessage(message, peer);
                    }
                    break;
                case MessageType.Async:
                    if (message.SubCode.HasValue && _eventHandlerSubList.ContainsKey(message.SubCode.Value))
                    {
                        _eventHandlerSubList[message.SubCode.Value].HandleMessage(message, peer);
                        handled = true;
                    }
                    else if (!message.SubCode.HasValue && _eventHandlerList.ContainsKey(message.Code))
                    {
                        _eventHandlerList[message.Code].HandleMessage(message, peer);
                        handled = true;
                    }
                    else
                    {
                        _defaultEventHander.HandleMessage(message, peer);
                    }
                    break;
            }

            return handled;
        }
    }
}
