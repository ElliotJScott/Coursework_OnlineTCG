using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace CourseworkServer
{
    public delegate void OnConnect(Client user, object sender);
    public delegate void OnDataReceived(Client sender, byte[] data);

    class Server
    {

        public Listener listener;
        public ActionQueue queue = new ActionQueue();
        public DatabaseHandler dbHandler = new DatabaseHandler();
        public static Server server;
        public List<Client> connectedClients;
        public const int port = 1337;
        MemoryStream readStream;
        MemoryStream writeStream;
        BinaryReader reader;
        BinaryWriter writer;
        static bool laptopConnection;
        const string laptopConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Lisa\\Source\\Repos\\Coursework\\Coursework Server\\CourseworkDB.mdf\";Integrated Security=True"; //Enter this later
        const string desktopConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Users\\Robert\\Source\\Repos\\Coursework\\Coursework Server\\CourseworkDB.mdf\";Integrated Security=True;User Instance=True";
        public static string connectionString;

            static void Main(string[] args)
        {
            #region Change this before hand-in
            Console.WriteLine("Use Laptop or Desktop connection string? L/D");
            switch (Console.Read())
            {
                case 'l':
                case 'L':
                    connectionString = laptopConnectionString;
                    laptopConnection = true;
                    break;
                case 'd':
                case 'D':
                    connectionString = desktopConnectionString;
                    laptopConnection = false;
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    Main(args);
                    break;
            }
            #endregion
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
            listener.userAdded += new OnConnect(listener_userAdded);
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
        public void listener_userAdded(Client user, object sender)
        {
            Console.WriteLine("Adding user to list of users");
            user.DataReceivedEvent += new OnDataReceived(user_DataReceived);
            user.DisconnectEvent += new OnConnect(user_UserDisconnected);

            connectedClients.Add(user);
        }

        public void user_UserDisconnected(Client user, object sender)
        {
            Console.WriteLine("Removing user");
            connectedClients.Remove(user);
        }
        private void ExecuteCommand(string s)
        {
            string[] splitted = s.Split(' ');
            switch (splitted[0])
            {
                case "/sql":
                    string genericSQLString = "";
                    for (int i = 1; i < splitted.Length; i++) genericSQLString += " " + splitted[i];
                    try
                    {
                        Console.WriteLine(dbHandler.DoSQLCommand(genericSQLString.Trim()) + " row(s) affected");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error encountered: " + e);
                    }
                    break;
                case "/sqlQuery":
                    string sqlQuery = "";
                    for (int i = 1; i < splitted.Length; i++) sqlQuery += " " + splitted[i];
                    try
                    {
                        DataTable table = dbHandler.DoSQLQuery(sqlQuery);
                        foreach (DataRow row in table.Rows)
                        {
                            for (int i = 0; i < table.Columns.Count; i++)
                            {
                                Console.Write(row[i].ToString() + " ");
                            }
                            Console.WriteLine();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error encountered: " + e);
                    }
                    break;
                    
                case "/checkCredentials":
                    //bool b = dbHandler.CheckLoginCredentials(splitted[1], splitted[2]);
                    //if (b) Console.WriteLine("Credentials valid");
                    //else Console.WriteLine("Invalid credentials");
                    break;
                case "/close":
                    Environment.Exit(0);
                    break;
                case "/help":
                    Console.WriteLine("/sql <command> : Executes the command given and prints the number of rows affected");
                    Console.WriteLine("/checkCredentials <Username> <PasswordHash> : Checks to see if the given user exists in the DB");
                    Console.WriteLine("/help : Prints all usable commands to the console");
                    Console.WriteLine("/close : Exits the program safely");
                    break;
                default:
                    Console.WriteLine("Command not found. Enter /help for all commands");
                    break;
            }
        }
        private void user_DataReceived(Client sender, byte[] data)
        {
            Protocol p = (Protocol)data[0];
            
            Console.WriteLine(p);
            foreach (byte b in data) Console.Write(" " + b);
            //SendData(data, sender);
            char[] chars = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                chars[i] = (char)data[i];
            }
            string s = new string(chars);
            Console.WriteLine(s); 
            
            switch (p)
            {
                case Protocol.CreateAccount:
                    queue.Enqueue(new ActionItem(Operation.AddNewAccount, s, sender));
                    break;
                case Protocol.LogIn:
                    queue.Enqueue(new ActionItem(Operation.CheckCredentials, s, sender));
                    break;
            }
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
