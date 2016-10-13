﻿using System;
using System.Collections.Generic;
using System.IO;
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
        MemoryStream readStream;
        MemoryStream writeStream;
        BinaryReader reader;
        BinaryWriter writer;

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

            readStream = new MemoryStream();
            writeStream = new MemoryStream();
            reader = new BinaryReader(readStream);
            writer = new BinaryWriter(writeStream);
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
        private void listener_userAdded(object sender, Client user)
        {

            user.DataReceivedEvent += new OnDataReceived(user_DataReceived);
            user.DisconnectEvent += new OnConnect(user_UserDisconnected);

            connectedClients.Add(user);
        }

        private void user_UserDisconnected(Client user, object sender)
        {

            connectedClients.Remove(user);
        }

        private void user_DataReceived(Client sender, byte[] data)
        {
            writeStream.Position = 0;


            writer.Write(sender.id);
            writer.Write(sender.ip);
            data = CombineData(data, writeStream);
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

        private byte[] CombineData(byte[] data, MemoryStream ms)
        {
            byte[] result = GetDataFromMemoryStream(ms);

            byte[] combinedData = new byte[data.Length + result.Length];

            for (int i = 0; i < data.Length; i++)
            {
                combinedData[i] = data[i];
            }

            for (int j = data.Length; j < data.Length + result.Length; j++)
            {
                combinedData[j] = result[j - data.Length];
            }

            return combinedData;
        }
    }
}