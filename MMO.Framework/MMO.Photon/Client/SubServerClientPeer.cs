using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMO.Framework;
using MMO.Photon.Application;

namespace MMO.Photon.Client
{
    public class SubServerClientPeer : IClient
    {
        private readonly Guid _peerId;
        private readonly Dictionary<Type, IClientData> _clientData = new Dictionary<Type, IClientData>();
        private readonly PhotonApplication _server;

        public Guid PeerId { get { return _peerId; } }

        public delegate SubServerClientPeer Factory(Guid peerId);

        public SubServerClientPeer(IEnumerable<IClientData> clientData, PhotonApplication application, Guid peerId)
        {
            _peerId = peerId;
            _server = application;
            foreach (var data in clientData)
            {
                _clientData.Add(data.GetType(), data);
            }
            
        }

        public T ClientData<T>() where T : class, IClientData
        {
            IClientData result;
            _clientData.TryGetValue(typeof (T), out result);
            if (result != null)
            {
                return result as T;
            }
            return null;
        }
    }
}
