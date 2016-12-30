using System;
using System.Net;
using System.Net.Sockets;

namespace CourseworkServer
{
    class Listener
    {
        public TcpListener listener;
        public event OnConnect userAdded;
        public const int port = 1337;
        public Listener()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptClient, null);
        }
        public void AcceptClient(IAsyncResult a)
        {
            TcpClient client = listener.EndAcceptTcpClient(a);
            Client cl = new Client(client);
            userAdded?.Invoke(cl);
            listener.BeginAcceptTcpClient(AcceptClient, null);
        }

    }
}
