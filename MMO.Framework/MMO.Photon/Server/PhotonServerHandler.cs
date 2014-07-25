using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MMO.Framework;

using MMO.Photon.Application;
using ExitGames.Logging;

namespace MMO.Photon.Server
{
    public abstract class PhotonServerHandler : IHandler<PhotonServerPeer>
    {
        public abstract MessageType Type { get; }
        public abstract byte Code { get; }
        public abstract int? SubCode { get; }
        protected PhotonApplication Server;
        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public PhotonServerHandler(PhotonApplication application)
        {
            Server = application;
        }

        public bool HandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            //Log.Debug(this.GetType().Name);
            // TODO remove
            try
            {
                OnHandleMessage(message, serverPeer);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return true;
        }

        protected abstract bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer);

    }
}
