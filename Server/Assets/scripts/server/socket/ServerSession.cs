using System.Net.Sockets;

namespace nsocket
{
    public class ServerSession
    {
        private string _ID;
        public string ID
        {
            set { _ID = value; }
            get { return _ID; }
        }

        public SocketPackMgr _SocketPackMgr;

        public ServerSession(string ipPort, Socket socket, NetworkStream networkStream)
        {
            ID = ipPort;
            _SocketPackMgr = new SocketPackMgr(socket, networkStream);
        }

    }
}