using System;
using System.Net;
using System.Net.Sockets;

namespace CourseworkServer
{
    class Listener
    {
        public TcpListener listener;
        public event OnConnect userAddedEvent;
        public Listener()
        {
            listener = new TcpListener(IPAddress.Any, 1337);
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptClient, null);
        }
        public void AcceptClient(IAsyncResult a)
        {
            TcpClient client = listener.EndAcceptTcpClient(a);
            Client cl = new Client(client, Server.server.getAvailableID());

            if (userAddedEvent != null)
            {
                userAddedEvent(cl, this);
            }
            Server.server.connectedClients.Add(cl);
            listener.BeginAcceptTcpClient(AcceptClient, null);
        }

    }
}
