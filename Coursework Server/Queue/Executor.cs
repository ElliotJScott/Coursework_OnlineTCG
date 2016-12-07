using System;
using System.Collections.Generic;
using System.Data;
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
                        DataTable table = Server.server.dbHandler.DoSQLQuery("select * from Accounts where Username = '" + usernameAndPasswordHash[0] + "'");
                        int numInstances = table.Rows.Count;
                        if (numInstances == 0)
                        {
                            int numRowsAffectedAddAccount = Server.server.dbHandler.DoSQLCommand("INSERT INTO Accounts VALUES('" + usernameAndPasswordHash[0] + "', '" + usernameAndPasswordHash[1] + "', 1000, 0)");
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
                        DataTable table = Server.server.dbHandler.DoSQLQuery("select * from Accounts where Username = '" + usernameAndPasswordHash[0] + "' AND PasswordHash = '" + usernameAndPasswordHash[1] + "'");
                        int numInstances = table.Rows.Count;
                        if (numInstances == 1)
                        {
                            currentItem.sender.SendData(new byte[] { (byte)Protocol.GoodCredentials });
                            int index = Server.server.connectedClients.IndexOf(currentItem.sender);
                            Server.server.connectedClients[index].userName = usernameAndPasswordHash[0];
                            Server.server.connectedClients[index].status = Status.Online;
                        }
                        else currentItem.sender.SendData(new byte[] { (byte)Protocol.BadCredentials });
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
            }
        }
    }
}
