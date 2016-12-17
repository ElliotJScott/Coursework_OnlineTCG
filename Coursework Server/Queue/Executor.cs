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
        public static void TransmitObjectArray(object[] o, Client c, Protocol p)
        {
            string s = "";
            foreach (object a in o)
            {
                s += a + "|";
            }
            s = s.Remove(s.Length - 1);
            Console.WriteLine(s);
            byte[] b = Server.addProtocolToArray(Server.toByteArray(s), p);
            c.SendData(b);
        }
        public void Execute()
        {
            switch (currentItem.operation)
            {
                #region AddNewAccount
                case Operation.AddNewAccount:
                    {
                        string[] usernameAndPasswordHash = ((string)currentItem.data).Substring(2).Split('|');
                        object[][] data = Server.server.dbHandler.DoParameterizedSQLQuery("select count(*) from Accounts where Username = @p1", usernameAndPasswordHash[0]);
                        if ((int)data[0][0] == 0)
                        {
                            int numRowsAffectedAddAccount = Server.server.dbHandler.DoParameterizedSQLCommand("INSERT INTO Accounts VALUES(@p1, @p2, 1000, 0)", usernameAndPasswordHash[0], usernameAndPasswordHash[1]);
                            Console.WriteLine(numRowsAffectedAddAccount + " row(s) affected");
                            DefaultDeckBuilder.AddDefaultDeckToPlayer(usernameAndPasswordHash[0]);
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
                        object[][] data = Server.server.dbHandler.DoParameterizedSQLQuery("select count(*) from Accounts where Username = @p1 and PasswordHash = @p2", usernameAndPasswordHash[0], usernameAndPasswordHash[1]);
                        if ((int)data[0][0] == 0)
                        {
                            currentItem.sender.SendData(new byte[] { (byte)Protocol.BadCredentials });
                        }
                        else if (Server.server.usernameInUse(usernameAndPasswordHash[0]))
                        {
                            currentItem.sender.SendData(new byte[] { (byte)Protocol.LoggedIn });
                        }
                        else
                        {
                            Server.server.queue.Enqueue(new ActionItem(Operation.TransmitDBData, null, currentItem.sender));
                            currentItem.sender.userName = usernameAndPasswordHash[0];
                            currentItem.sender.status = Status.Online;
                            Server.server.queue.Enqueue(new ActionItem(Operation.GetPlayerElo, null, currentItem.sender));
                            currentItem.sender.SendData(new byte[] { (byte)Protocol.GoodCredentials });
                        }
                    }
                    break;
                #endregion
                #region CheckFriendStatus
                case Operation.CheckFriendStatus:
                    {
                        string username = ((string)currentItem.data).Substring(2);
                        currentItem.sender.friends.Add(username);
                    }
                    break;
                #endregion
                #region AddToQueue
                case Operation.AddToQueue:
                    {
                        int qs = Convert.ToInt32(((string)currentItem.data).Substring(2));
                        currentItem.sender.status = Status.InQueue;
                        currentItem.sender.queueStatus = qs;
                        currentItem.sender.queuetime = 0;
                    }
                    break;
                #endregion
                #region GetPlayerElo
                case Operation.GetPlayerElo:
                    {
                        string username = currentItem.sender.userName;
                        object[][] data = Server.server.dbHandler.DoParameterizedSQLQuery("select elo from accounts where username = @p1", username);
                        int elo = (int)data[0][0];
                        currentItem.sender.elo = elo;
                    }
                    break;
                #endregion
                #region TransmitDBData
                case Operation.TransmitDBData:
                    {
                        object[][] allCards = Server.server.dbHandler.DoParameterizedSQLQuery("select cardname, cardtype, cardrarity, cardattack, carddefence, cardcost from cards");
                        object[][] allEffects = Server.server.dbHandler.DoParameterizedSQLQuery("select effectname, effectdescription, effectcolour from effect");
                        object[][] allCardEffects = Server.server.dbHandler.DoParameterizedSQLQuery("select cards.cardname, effect.effectname from cardeffect join cards on cards.cardid = cardeffect.cardid join effect on effect.effectid = cardeffect.effectid");
                        object[][] allDecks = Server.server.dbHandler.DoParameterizedSQLQuery("select decks.deckid, decks.allcards from Accounts join decks on decks.accountid = accounts.accountid and accounts.username = @p1", currentItem.sender.userName);
                        foreach (object[] o in allDecks)
                        {
                            {
                                Console.WriteLine("Transmitting deck data");
                                TransmitObjectArray(o, currentItem.sender, Protocol.DeckData);
                            }
                        }
                        Console.WriteLine("Transmitting all card data");
                        foreach (object[] o in allCards)
                        {
                            TransmitObjectArray(o, currentItem.sender, Protocol.CardData);
                        }
                        Console.WriteLine("Transmitting all effect data");
                        foreach (object[] o in allEffects)
                        {
                            TransmitObjectArray(o, currentItem.sender, Protocol.EffectData);
                        }
                        Console.WriteLine("Transmitting all cardeffect data");
                        foreach (object[] o in allCardEffects)
                        {
                            TransmitObjectArray(o, currentItem.sender, Protocol.CardEffect);
                        }
                        foreach (object[] o in allDecks)
                        {
                            int id = (int)o[0];
                            object[][] deckContents = Server.server.dbHandler.DoParameterizedSQLQuery("select cards.cardname, deckcards.deckid, deckcards.cardquantity from deckcards join cards on cards.cardid = deckcards.cardid where deckid = @p1", id);
                            Console.WriteLine("Transmitting all deckcard data");
                            foreach (object[] a in deckContents)
                            {
                                TransmitObjectArray(a, currentItem.sender, Protocol.DeckCardsData);
                            }
                        }
                        Console.WriteLine("All data transmitted");
                    }
                    break;
                    #endregion
            }
        }
    }
}
