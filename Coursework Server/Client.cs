using System;
using System.Net.Sockets;

namespace CourseworkServer
{
    public class Client
    {

        TcpClient tcpClient; //The tcpclient used for receiving data from the end user
        public string ip; //The ip address for the client
        public int id; //The unique identifier for this particular client connected to the server
        public const int bufferSize = 10000; //The size of the buffer
        byte[] buffer = new byte[bufferSize]; //The buffer that data is written into when data is received from the end user
        public event OnConnect DisconnectEvent;
        public event OnDataReceived DataReceivedEvent;

        public Client(TcpClient c, int i)
        {
            tcpClient = c;
            ip = c.Client.RemoteEndPoint.ToString();
            tcpClient.NoDelay = true;
            id = i;
            tcpClient.GetStream().BeginRead(buffer, 0, bufferSize, DataReceived, null); //Listen for new data and call the DataReceived method when data has been received
        }
        public void Disconnect()
        {
            DisconnectEvent(this, this);
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
                Console.WriteLine("Client {0}:  {1}\n{2}", ip, "Bad data", "Disconnecting");
                return;
            }
            for (int i = 0; i < length; i++)
            {
                d[i] = buffer[i];
            }
            tcpClient.GetStream().BeginRead(buffer, 0, bufferSize, DataReceived, null);
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
            else return id == ((Client)obj).id;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}