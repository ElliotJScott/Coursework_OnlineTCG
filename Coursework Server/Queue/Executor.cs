using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
    class Executor
    {
        public ActionItem currentItem = null;
        public Executor()
        {
            Console.WriteLine("Creating executor");
        }
        public void Execute()
        {
            switch (currentItem.operation)
            {
                #region AddNewAccount
                case Operation.AddNewAccount:
                    {
                        string[] usernameAndPasswordHash = ((string)currentItem.data).Substring(2).Split('|');
                        //DataTable table = Server.server.dbHandler.DoSQLQuery("select * from Accounts where Username = '" + usernameAndPasswordHash[0] + "'");
                        object[][] data = Server.server.dbHandler.DoParameterizedSQLQuery("select * from Accounts where Username = @p1", usernameAndPasswordHash[0]);                    
                        if (data.GetLength(1) == 0)
                        {
                            int numRowsAffectedAddAccount = Server.server.dbHandler.DoParameterizedSQLCommand("INSERT INTO Accounts VALUES(@p1, @p2, 1000, 0)", usernameAndPasswordHash[0], usernameAndPasswordHash[1]);
                            Console.WriteLine(numRowsAffectedAddAccount + " row(s) affected");
                        }
                        else
                        {
                            currentItem.sender.SendData(new byte[] { (byte)Protocol.UsernameTaken });
                        }
                    }
                    break;
                #endregion
                #region CheckCredentials
                case Operation.CheckCredentials:
                    {
                        string[] usernameAndPasswordHash = ((string)currentItem.data).Substring(2).Split('|');
                        object[][] data = Server.server.dbHandler.DoParameterizedSQLQuery("select * from Accounts where Username = @p1 and PasswordHash = @p2", usernameAndPasswordHash[0], usernameAndPasswordHash[1]);
                        if (data.Length == 0)
                        {
                            currentItem.sender.SendData(new byte[] { (byte)Protocol.BadCredentials });
                        }
                        else if (Server.server.usernameInUse(usernameAndPasswordHash[0]))
                        {
                            currentItem.sender.SendData(new byte[] { (byte)Protocol.LoggedIn });
                        }
                        else
                        {
                            currentItem.sender.SendData(new byte[] { (byte)Protocol.GoodCredentials });
                            int index = Server.server.connectedClients.IndexOf(currentItem.sender);
                            Server.server.connectedClients[index].userName = usernameAndPasswordHash[0];
                            Server.server.connectedClients[index].status = Status.Online;
                            Server.server.queue.Enqueue(new ActionItem(Operation.GetPlayerElo, null, currentItem.sender));
                        }
                    }
                    break;
                #endregion
                #region CheckFriendStatus
                case Operation.CheckFriendStatus:
                    {
                        string username = ((string)currentItem.data).Substring(2);
                        int index = Server.server.connectedClients.IndexOf(currentItem.sender);
                        Server.server.connectedClients[index].friends.Add(username);
                    }
                    break;
                #endregion
                #region AddToQueue
                case Operation.AddToQueue:
                    {
                        int qs = Convert.ToInt32(((string)currentItem.data).Substring(2));
                        int index = Server.server.connectedClients.IndexOf(currentItem.sender);
                        Server.server.connectedClients[index].status = Status.InQueue;
                        Server.server.connectedClients[index].queueStatus = qs;
                    }
                    break;
                #endregion
                #region GetPlayerElo
                case Operation.GetPlayerElo:
                    {
                        string username = currentItem.sender.userName;
                        object[][] data = Server.server.dbHandler.DoParameterizedSQLQuery("select elo from accounts where username = @p1", username);
                        int elo = (int)data[0][0];
                        Server.server.connectedClients[Server.server.GetClientIndex(username)].elo = elo;
                    }
                    break;                 
                    #endregion
            }
        }
    }
}
