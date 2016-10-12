using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
    class Listener
    {
        TcpListener listener;
        public Listener()
        {
            listener = new TcpListener(System.Net.IPAddress.Any, 1337);
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptClient, null);
        }
        public void AcceptClient(IAsyncResult a)
        {
            TcpClient client = listener.EndAcceptTcpClient(a);
            Server.server.connectedClients.Add(new Client(client, id));
            listener.BeginAcceptTcpClient(AcceptClient, null);
        }
    }
}
