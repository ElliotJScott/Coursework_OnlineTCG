using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;

namespace CourseworkServer
{
    public delegate void OnConnect(Client user);
    public delegate void OnDataReceived(Client sender, byte[] data);

    class Server
    {

        public Listener listener;
        public ActionQueue queue = new ActionQueue();
        public DatabaseHandler dbHandler = new DatabaseHandler();
        public static Server server;
        public List<Client> connectedClients;
        public List<Match> currentMatches = new List<Match>();
        MemoryStream readStream;
        MemoryStream writeStream;
        BinaryReader reader;
        BinaryWriter writer;
        public Random rng = new Random();
        const string laptopConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\Lisa\\Source\\Repos\\Coursework\\Coursework Server\\CourseworkDB.mdf\";Integrated Security=True";
        const string desktopConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=\"C:\\Users\\Robert\\Source\\Repos\\Coursework\\Coursework Server\\CourseworkDB.mdf\";Integrated Security=True;User Instance=True";
        public static string connectionString; //The correct connection string out of the two available ones
        static bool isReadingForCommand = false;
        Random rnd = new Random();

        /// <summary>
        /// The entry point of the program
        /// </summary>
        /// <param name="args">Has no effect</param>
        static void Main(string[] args)
        {
            #region Change this before hand-in: this is so that it works on both my desktop and laptop
            Console.WriteLine("Use Laptop or Desktop connection string? L/D");
            switch (Console.ReadLine())
            {
                case "l":
                case "L":
                    connectionString = laptopConnectionString;
                    break;
                case "d":
                case "D":
                    connectionString = desktopConnectionString;
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    Main(args);
                    break;
            }
            #endregion
            server = new Server();
            Console.Clear();
            Console.WriteLine("Server online");
            while (true)
            {
                if (!isReadingForCommand)
                {
                    Thread t = new Thread(new ThreadStart(ReadCommand));
                    t.Start();
                    isReadingForCommand = true;
                }
                Thread t2 = new Thread(new ThreadStart(UpdateQueues));
                t2.Start();
                Thread.Sleep(1000);

            }
        }
        /// <summary>
        /// Gets the range of elos that a player could face
        /// </summary>
        /// <param name="t">The time that the player has been connected to the server</param>
        /// <returns>The range of elos</returns>
        public static int GetRangeFromTime(double t)
        {
            return (int)(50 * Math.Atan((t / 15) - 6) + 120);
        }
        /// <summary>
        /// Checks if there is a valid match that could be created out of all of the players queued
        /// </summary>
        public static void UpdateQueues()
        {
            for (int i = 0; i < server.connectedClients.Count; i++)
            {
                if (server.connectedClients[i].status == Status.InQueue)
                {
                    Console.WriteLine(i + ": " + server.connectedClients[i].userName + " is in queue. Time: " + server.connectedClients[i].queuetime + ". Range = " + GetRangeFromTime(server.connectedClients[i].queuetime));
                    server.connectedClients[i].queuetime++;
                    for (int j = 0; j < i; j++)
                    {
                        if (server.connectedClients[j].status == Status.InQueue)
                        {
                            int range = Math.Min(GetRangeFromTime(server.connectedClients[i].queuetime), GetRangeFromTime(server.connectedClients[j].queuetime));
                            if (Math.Abs(server.connectedClients[i].elo - server.connectedClients[j].elo) <= range)
                            {
                                CreateMatch(server.connectedClients[i], server.connectedClients[j]);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Creates a match between the two connected players
        /// </summary>
        /// <param name="a">One player</param>
        /// <param name="b">The other player</param>
        public static void CreateMatch(Client a, Client b)
        {
            Match m = new Match(a, b);
            server.currentMatches.Add(m);
            int x = server.rng.Next(2);
            a.SendData(addProtocolToArray(toByteArray(x + b.userName), Protocol.EnterMatch));
            b.SendData(addProtocolToArray(toByteArray((1 - x) + a.userName), Protocol.EnterMatch));
            a.status = Status.InGame;
            b.status = Status.InGame;
        }
        /// <summary>
        /// Adds the protocol to the beginning of the byte array
        /// </summary>
        /// <param name="b">The initial byte array</param>
        /// <param name="p">The protocol to add</param>
        /// <returns>The byte array after the protocol is added</returns>
        public static byte[] addProtocolToArray(byte[] b, Protocol p)
        {
            byte[] e = new byte[b.Length + 1];
            e[0] = (byte)p;
            for (int i = 0; i < b.Length; i++)
            {
                e[i + 1] = b[i];
            }
            return e;
        }
        /// <summary>
        /// Converts a string to an array of bytes
        /// </summary>
        /// <param name="s">The input string</param>
        /// <returns>The string as a byte array</returns>
        public static byte[] toByteArray(string s)
        {
            char[] c = s.ToCharArray();
            byte[] b = new byte[c.Length];
            for (int i = 0; i < c.Length; i++)
            {
                b[i] = (byte)c[i];
            }
            return b;
        }
        /// <summary>
        /// Executes a command read from the console. This will possibly go before hand-in
        /// </summary>
        public static void ReadCommand()
        {
            server.ExecuteCommand(Console.ReadLine());
            isReadingForCommand = false;
        }
        /// <summary>
        /// Creates a new instance of the server and sets it up.
        /// </summary>
        public Server()
        {
            if (!File.Exists("Messages.txt"))
            {
                File.Create("Messages.txt");
            }
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
        /// <summary>
        /// Called whenever a client is added to the server. Adds events for when the client does something.
        /// </summary>
        /// <param name="user">The connected client</param>
        public void listener_userAdded(Client user)
        {
            Console.WriteLine("Adding user to list of users");
            user.DataReceivedEvent += new OnDataReceived(user_DataReceived);
            user.DisconnectEvent += new OnConnect(user_UserDisconnected);

            connectedClients.Add(user);
        }
        /// <summary>
        /// Called when a user disconnects from the server
        /// </summary>
        /// <param name="user">The user that disconnected</param>
        public void user_UserDisconnected(Client user)
        {
            Console.WriteLine("Removing user");
            connectedClients.Remove(user);
        }
        /// <summary>
        /// Executes the given console command
        /// </summary>
        /// <param name="s">The command to execute</param>
        private void ExecuteCommand(string s)
        {
            string[] splitted = s.Split(' ');
            switch (splitted[0])
            {
                case "/sql": //Executes a sql command and prints the number of rows affected
                    string genericSQLString = "";
                    for (int i = 1; i < splitted.Length; i++) genericSQLString += " " + splitted[i];
                    try
                    {
                        Console.WriteLine(dbHandler.DoParameterizedSQLCommand(genericSQLString.Trim()) + " row(s) affected");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error encountered: " + e);
                    }
                    break;
                case "/sqlQuery": //Executes a sql query and prints the result
                    string sqlQuery = "";
                    for (int i = 1; i < splitted.Length; i++) sqlQuery += " " + splitted[i];
                    try
                    {
                        object[][] data = dbHandler.DoParameterizedSQLQuery(sqlQuery);
                        for (int i = 0; i < data.Length; i++)
                        {
                            for (int j = 0; j < data[i].Length; j++)
                            {
                                Console.Write(data[i][j] + " ");
                            }
                            Console.WriteLine();

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error encountered: " + e);
                    }
                    finally
                    {
                        if (reader != null) reader.Close();
                    }
                    break;

                case "/close": //Closes the server
                    Environment.Exit(0);
                    break;
                case "/testDataTransmission": //Tests connection to every connected client
                    foreach (Client c in connectedClients)
                    {
                        for (int i = 0; i < 1000; i++) c.SendData(addProtocolToArray(toByteArray("test string data " + i), Protocol.DataTransmissionTest));
                    }
                    break;
                default:
                    Console.WriteLine("Command not found. Enter /help for all commands");
                    break;
            }
        }
        /// <summary>
        /// Called whenever data is received from a client
        /// </summary>
        /// <param name="sender">The client sending the data</param>
        /// <param name="data">The data sent</param>
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
                    queue.Enqueue(new ActionItem(Operation.AddNewAccount, s, sender, Priority.high));
                    break;
                case Protocol.LogIn:
                    queue.Enqueue(new ActionItem(Operation.CheckCredentials, s, sender));
                    break;
                case Protocol.AddToQueue:
                    queue.Enqueue(new ActionItem(Operation.AddToQueue, s, sender));
                    break;
                case Protocol.WonGame:
                    queue.Enqueue(new ActionItem(Operation.CalculateEloCoinChanges, s, sender));
                    break;
                case Protocol.BasicPack:
                    queue.Enqueue(new ActionItem(Operation.BasicPack, s, sender));
                    break;
                case Protocol.PremiumPack:
                    queue.Enqueue(new ActionItem(Operation.PremiumPack, s, sender));
                    break;
                case Protocol.UpdatedDecks:
                    queue.Enqueue(new ActionItem(Operation.ClearDBDeckCards, s.Substring(1), sender));
                    break;
                case Protocol.NewDeckCards:
                    queue.Enqueue(new ActionItem(Operation.AddCardToDeck, s.Substring(1), sender));
                    break;
                case Protocol.ControlUnit:
                case Protocol.DiscardFromDeck:
                case Protocol.DiscardFromUpgradeDeck:
                case Protocol.KillUnit:
                case Protocol.NoCardsInDeck:
                case Protocol.NoCardsInUpgradeDeck:
                case Protocol.PlayTech:
                case Protocol.PlayUnitFromDeck:
                case Protocol.PlayUpgrade:
                case Protocol.ReplaceUnit:
                case Protocol.ReturnUnit:
                case Protocol.PlayUnit:
                case Protocol.DiscardTech:
                case Protocol.EquipUpgrade:
                case Protocol.AttackWithUnit:
                case Protocol.DefendWithUnit:
                case Protocol.NoCounter:
                case Protocol.EndTurn:
                case Protocol.BeginSelection:
                case Protocol.EndSelection:
                case Protocol.AddCardToEnemyHand:
                case Protocol.AddToEnemyFromDiscard:
                case Protocol.Artillery:
                case Protocol.DeathInHonour:
                case Protocol.RemoveCardFromEnemyHand:
                case Protocol.HealHalf:
                case Protocol.HealFull:
                case Protocol.PowerExtraction:
                case Protocol.AddCardFromDiscard:
                case Protocol.ReturnUnitToHand:
                    //If the data does not need to be processed by the server
                    GetOpponent(sender).SendData(data);
                    break;
            }
        }
        /// <summary>
        /// Gets the opponent of a player in a match
        /// </summary>
        /// <param name="c">The player</param>
        /// <returns>The player's opponent</returns>
        public Client GetOpponent(Client c)
        {
            string opponentName = null;
            foreach (Match m in currentMatches)
            {
                if (m.players[0] == c.userName) opponentName = m.players[1];
                else if (m.players[1] == c.userName) opponentName = m.players[0];
            }
            if (opponentName == null) throw new ArgumentException();
            else return GetClient(opponentName);
        }
        /// <summary>
        /// Gets the data from the given memory stream
        /// </summary>
        /// <param name="ms">The memory stream</param>
        /// <returns>The data</returns>
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
        /// <summary>
        /// Gets the status of the given user
        /// </summary>
        /// <param name="s">The user's username</param>
        /// <returns>The user's status</returns>
        public Status StatusOf(string s)
        {
            foreach (Client c in connectedClients)
            {
                if (c.userName == s) return c.status;
            }
            return Status.Offline;
        }
        /// <summary>
        /// Gets the index of the player in the list of clients
        /// </summary>
        /// <param name="s">The username</param>
        /// <returns>The index of the client</returns>
        public int GetClientIndex(string s)
        {
            for (int i = 0; i < connectedClients.Count; i++)
            {
                if (connectedClients[i].userName == s) return i;
            }
            throw new ArgumentException(s);
        }
        /// <summary>
        /// Checks whether the given username is in use
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>Whether the username is in use or not</returns>
        public bool usernameInUse(string username)
        {
            foreach (Client c in connectedClients)
            {
                if (c.userName == username) return true;
            }
            return false;
        }
        /// <summary>
        /// Gets the connected client with the given username
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>The client with that username</returns>
        public Client GetClient(string username)
        {
            foreach (Client c in connectedClients)
            {
                if (c.userName == username) return c;
            }
            throw new ArgumentException(); //This will get called if I use the test ingameform
        }
        public string[] GetPackCards(double uncommonChance, double veryRareChance)
        {
            object[][] commons = dbHandler.DoParameterizedSQLQuery("select cardname from cards where cardrarity = 0");
            object[][] uncommons = dbHandler.DoParameterizedSQLQuery("select cardname from cards where cardrarity = 1");
            object[][] rares = dbHandler.DoParameterizedSQLQuery("select cardname from cards where cardrarity = 2");
            object[][] veryRares = dbHandler.DoParameterizedSQLQuery("select cardname from cards where cardrarity = 3");
            int uChance = (int)(100 * uncommonChance);
            int vrChance = (int)(100 * veryRareChance);
            string[] output = new string[5];
            for (int i = 0; i < 4; i++)
            {
                bool uc = rnd.Next(99) < uChance;
                object[][] o;
                if (uc) o = uncommons;
                else o = commons;
                int index = rnd.Next(o.GetLength(0));
                object[] f = o[index];
                output[i] = (string)f[0];
            }
            bool vr = rnd.Next(99) < vrChance;
            object[][] l;
            if (vr) l = veryRares;
            else l = rares;
            int ind = rnd.Next(l.GetLength(0));
            object[] j = l[ind];
            output[4] = (string)j[0];
            return output;
        }
        public void UpdatePackCardsOnDB(Client sender, string[] cardNames)
        {
            object[][] d = dbHandler.DoParameterizedSQLQuery("select decks.deckid from accounts join decks on decks.accountid = accounts.accountid and accounts.username = @p1 and decks.allcards = 1", sender.userName);
            int deckid = (int)d[0][0];
            foreach (string s in cardNames)
            {
                object[][] p = dbHandler.DoParameterizedSQLQuery("select cardid from cards where cardname = @p1", s);
                int cardid = (int)p[0][0];
                object[][] c = dbHandler.DoParameterizedSQLQuery("select count(*) from deckcards where cardid = @p1 and deckid = @p2", cardid, deckid);
                int count = (int)c[0][0];
                if (count == 0)
                {
                    dbHandler.DoParameterizedSQLCommand("insert into deckcards values(@p1, @p2, 1)", cardid, deckid);
                }
                else
                {
                    object[][] f = dbHandler.DoParameterizedSQLQuery("select cardquantity from deckcards where cardid = @p1 and deckid = @p2", cardid, deckid);
                    int num = (int)f[0][0];
                    dbHandler.DoParameterizedSQLCommand("update deckcards set cardquantity = @p1 where cardid = @p2 and deckid = @p3", num + 1, cardid, deckid);
                }
            }
        }
    }
}
