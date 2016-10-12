using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourseworkServer
{
    public delegate void OnConnect(Client user, object sender);
    public delegate void OnDataReceived(Client user, byte[] data);

    class Server
    {
        public Listener listener;
        public static Server server;
        public List<Client> connectedClients;
        public const int port = 1337;

        static void Main(string[] args)
        {
            server = new Server();
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public Server()
        {
            listener = new Listener();
            connectedClients = new List<Client>();
        }

        public int getAvailableID()
        {
            for (int i = 0; i <= connectedClients.Count; i++)
            {
                bool idTaken = false;
                foreach (Client c in connectedClients)
                {
                    if (c.id == i)
                    {
                        idTaken = true;
                        break;
                    }
                }
                if (!idTaken) return i;
            }
            throw new Exception("An unexpected error has occurred");
        }
    }
}
