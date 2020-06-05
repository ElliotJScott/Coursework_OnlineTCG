using System;
using System.Net;
using System.Net.Sockets;

namespace CourseworkServer
{
    class Listener
    {
        public TcpListener listener;
        public event OnConnect userAdded;
        public const int port = 1338;
        /// <summary>
        /// Creates a new listener to listen for any ip address on the port trying to connect
        /// </summary>
        public Listener()
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            listener.BeginAcceptTcpClient(AcceptClient, null);
        }
        /// <summary>
        /// This method is called when a new client connects
        /// </summary>
        /// <param name="a">This is used to get the client that connected</param>
        public void AcceptClient(IAsyncResult a)
        {
            TcpClient client = listener.EndAcceptTcpClient(a);
            Client cl = new Client(client);
            userAdded?.Invoke(cl);
            listener.BeginAcceptTcpClient(AcceptClient, null);
        }

    }
}
