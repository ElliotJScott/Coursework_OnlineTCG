using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace CourseworkServer
{
    /// <summary>
    /// This is the status of a player
    /// </summary>
    public enum Status
    {
        Offline,
        Online,
        InQueue,
        InGame
    }
    public class Client
    {
        public Status status = Status.Offline;
        TcpClient tcpClient; //The tcpclient used for receiving data from the end user
        public string ip; //The ip address for the client
        public const int bufferSize = 1000000; //The size of the buffer
        byte[] buffer = new byte[bufferSize]; //The buffer that data is written into when data is received from the end user
        public event OnConnect DisconnectEvent;
        public event OnDataReceived DataReceivedEvent;
        public string userName = "";
        public List<string> friends = new List<string>();
        public int queueStatus = 0;
        public int queuetime = 0;
        public int elo;
        public const byte transmissionDemarcator = (byte)'`';

        /// <summary>
        /// Creates a new client connected to the server.
        /// </summary>
        /// <param name="c">The client that has connected</param>
        public Client(TcpClient c)
        {
            tcpClient = c;
            ip = c.Client.RemoteEndPoint.ToString();
            tcpClient.NoDelay = true;
            tcpClient.GetStream().BeginRead(buffer, 0, bufferSize, DataReceived, null); //Listen for new data and call the DataReceived method when data has been received
        }
        /// <summary>
        /// Disconnect the player from the server
        /// </summary>
        public void Disconnect()
        {
            DisconnectEvent(this);
        }
        /// <summary>
        /// This method is called whenever data is received from 
        /// </summary>
        /// <param name="a">This is the result of the data reception</param>
        public void DataReceived(IAsyncResult a)
        {
            int length = 0; //The number of bytes of data that were received from the end user
            try
            {
                lock (tcpClient.GetStream())
                {
                    length = tcpClient.GetStream().EndRead(a); //Get the number of bytes read
                }

            }
            catch
            {
                Console.WriteLine("Error reading data");
            }
            byte[] d = new byte[length];
            if (length == 0) //If bad data was received from the client
            {
                Disconnect();
                Console.WriteLine("Client disconnecting");
                return;
            }
            for (int i = 0; i < length; i++)
            {
                d[i] = buffer[i];
                Console.WriteLine(d[i]);
            }
            tcpClient.GetStream().BeginRead(buffer, 0, bufferSize, DataReceived, null);
            DataReceivedEvent?.Invoke(this, d);

        }
        /// <summary>
        /// Send data to this client
        /// </summary>
        /// <param name="b">The data to send to the client</param>
        public void SendData(byte[] b)
        {
            try
            {
                lock (tcpClient.GetStream())
                {
                    byte[] a = new byte[b.Length + 2];
                    a[0] = transmissionDemarcator;
                    a[a.Length - 1] = transmissionDemarcator;
                    for (int i = 1; i < a.Length - 1; i++) a[i] = b[i - 1];

                    tcpClient.GetStream().BeginWrite(a, 0, a.Length, null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                SendData(b);
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            else return userName == ((Client)obj).userName;
        }
        public static bool operator ==(Client a, Client b) => a.Equals(b);       
        public static bool operator !=(Client a, Client b) => !(a == b);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// Notifies all friends of a change in status
        /// </summary>
        public void NotifyFriendsStatus()
        {
            foreach (string s in friends)
            {
                char[] c = s.ToCharArray();
                byte[] b = new byte[c.Length + 2];
                b[0] = (byte)Protocol.FriendStatus;
                for (int i = 1; i <= c.Length; i++) b[i] = (byte)c[i];
                b[c.Length + 1] = (byte)status;
                Server.server.connectedClients[Server.server.GetClientIndex(s)].SendData(b);
            }
        }

    }
}