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
                    string[] usernameAndPasswordHash1 = ((string)currentItem.data).Substring(2).Split('|');
                    DataTable table = Server.server.dbHandler.DoSQLQuery("select * from Accounts where Username = '" + usernameAndPasswordHash1[0] + "'");
                    int numInstances = table.Rows.Count;
                    if (numInstances == 0)
                    {
                        int numRowsAffectedAddAccount = Server.server.dbHandler.DoSQLCommand("INSERT INTO Accounts VALUES('" + usernameAndPasswordHash1[0] + "', '" + usernameAndPasswordHash1[1] + "', 1000, 0)");
                        Console.WriteLine(numRowsAffectedAddAccount + " row(s) affected");
                    }
                    else
                    {
                        currentItem.sender.SendData(new byte[] { (byte)Protocol.UsernameTaken});
                    }
                    break;
                #endregion
                #region CheckCredentials
                case Operation.CheckCredentials:
                    string[] usernameAndPasswordHash2 = ((string)currentItem.data).Substring(2).Split('|');
                    DataTable table2 = Server.server.dbHandler.DoSQLQuery("select * from Accounts where Username = '" + usernameAndPasswordHash2[0] + "' AND PasswordHash = '" + usernameAndPasswordHash2[1] + "'");
                    int numInstances2 = table2.Rows.Count;
                    if (numInstances2 == 1)
                    {
                        currentItem.sender.SendData(new byte[] { (byte)Protocol.GoodCredentials });
                        int index = Server.server.connectedClients.IndexOf(currentItem.sender);
                        Server.server.connectedClients[index].userName = usernameAndPasswordHash2[0];
                        Server.server.connectedClients[index].status = Status.Online;
                    }
                    else currentItem.sender.SendData(new byte[] { (byte)Protocol.BadCredentials });
                    break;
                #endregion
                #region CheckFriendStatus
                case Operation.CheckFriendStatus:
                    
                    break;
                #endregion
            }
        }
    }
}
