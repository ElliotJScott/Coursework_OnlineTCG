using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace CourseworkServer
{
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
        public const int bufferSize = 10000; //The size of the buffer
        byte[] buffer = new byte[bufferSize]; //The buffer that data is written into when data is received from the end user
        public event OnConnect DisconnectEvent;
        public event OnDataReceived DataReceivedEvent;
        public string userName = "";
        public List<string> friends = new List<string>();
        public int queueStatus = 0;
        public int queuetime = 0;
        public int elo;

        public Client(TcpClient c)
        {
            tcpClient = c;
            ip = c.Client.RemoteEndPoint.ToString();
            tcpClient.NoDelay = true;
            tcpClient.GetStream().BeginRead(buffer, 0, bufferSize, DataReceived, null); //Listen for new data and call the DataReceived method when data has been received
        }
        public void Disconnect()
        {
            DisconnectEvent(this);
        }
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
            byte[] d = new byte[length]; //An array to write the bytes into from the buffer where they were before. The buffer is used as the number of bytes needed is uncertain but now it is known.
            if (length == 0)
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

        public void SendData(byte[] b)
        {
            lock (tcpClient.GetStream())
            {
                tcpClient.GetStream().BeginWrite(b, 0, b.Length, null, null);
            }
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            else return userName == ((Client)obj).userName;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
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