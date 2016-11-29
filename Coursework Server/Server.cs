using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CourseworkServer
{
    public delegate void OnConnect(Client user, object sender);
    public delegate void OnDataReceived(Client user, byte[] data);

    class Server
    {
        public const int executionThreads = 10;
        public Executor[] executors = new Executor[executionThreads];
        public Listener listener;
        public Delegator delegator;
        public DatabaseHandler dbHandler = new DatabaseHandler();
        public static Server server;
        public List<Client> connectedClients;
        public const int port = 1337;
        MemoryStream readStream;
        MemoryStream writeStream;
        BinaryReader reader;
        BinaryWriter writer;

        static void Main(string[] args)
        {
            
            server = new Server();
            Console.WriteLine("Server online");
            while (true)
            {
                server.ExecuteCommand(Console.ReadLine());
            }
        }

        public Server()
        {
            Console.WriteLine("Initialising Server");
            listener = new Listener();
            connectedClients = new List<Client>();
            Console.WriteLine("Initialising writers and readers");
            readStream = new MemoryStream();
            writeStream = new MemoryStream();
            reader = new BinaryReader(readStream);
            writer = new BinaryWriter(writeStream);
            delegator = new Delegator();
            for (int i = 0; i < executionThreads; i++)
            {
                executors[i] = new Executor();
            }
        }

        public int getAvailableID()
        {
            Console.WriteLine("Searching for available ID for new connection");
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
            throw new Exception("An unexpected error has occurred. The program will now terminate");
        }
        private void listener_userAdded(object sender, Client user)
        {
            Console.WriteLine("Adding user to list of users");
            user.DataReceivedEvent += new OnDataReceived(user_DataReceived);
            user.DisconnectEvent += new OnConnect(user_UserDisconnected);

            connectedClients.Add(user);
        }

        private void user_UserDisconnected(Client user, object sender)
        {
            Console.WriteLine("Removing user");
            connectedClients.Remove(user);
        }
        private void ExecuteCommand(string s)
        {
            string[] splitted = s.Split(' ');
            switch (splitted[0])
            {
                case "/sendSQL":
                    string genericSQLString = "";
                    for (int i = 1; i < splitted.Length; i++) genericSQLString += splitted[i];
                    try
                    {
                        Console.WriteLine(dbHandler.ExecuteGenericSQL(genericSQLString));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error encountered: " + e);
                    }
                    break;
                case "/help":
                    Console.WriteLine("/sendSQL <String> : Executes a SQL command and prints the output to the console if relevant");
                    break;
                default:
                    Console.WriteLine("Command not found. Enter /help for all commands");
                    break;
            }
        }
        private void user_DataReceived(Client destination, byte[] data)
        {
            //SendData(data, sender);
        }

        private byte[] GetDataFromMemoryStream(MemoryStream ms)
        {
            byte[] result;

            lock (ms)
            {
                int bytesWritten = (int)ms.Position;
                result = new byte[bytesWritten];

                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
            }

            return result;
        }

    }
}
