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
        /// <summary>
        /// The constructor does no extra important things other than printing that the Executor has been created.
        /// </summary>
        public Executor()
        {
            Console.WriteLine("Creating executor");
        }
        /// <summary>
        /// Transmits the data given to the client given
        /// </summary>
        /// <param name="o">The data to send</param>
        /// <param name="c">The client to send the data to</param>
        /// <param name="p">The protocol that should be attached to the data</param>
        public static void TransmitObjectArray(object[] o, Client c, Protocol p)
        {
            string s = "";
            foreach (object a in o)
            {
                s += a + "|";
            }
            s = s.Remove(s.Length - 1);
            //Console.WriteLine(s);
            byte[] b = Server.addProtocolToArray(Server.toByteArray(s), p);
            c.SendData(b);
        }
        /// <summary>
        /// Executes the item that the executor has stored based on its operation.
        /// </summary>
        public void Execute()
        {
            switch (currentItem.operation)
            {
                #region AddNewAccount 
                case Operation.AddNewAccount: //Checks if the username sent is in use or not and returns the result to the sender. If the account does not exist it is created.
                    {
                        string[] usernameAndPasswordHash = ((string)currentItem.data).Substring(1).Split('|');
                        object[][] data = Server.server.dbHandler.DoParameterizedSQLQuery("select count(*) from Accounts where Username = @p1", usernameAndPasswordHash[0]);
                        if ((int)data[0][0] == 0)
                        {
                            int numRowsAffectedAddAccount = Server.server.dbHandler.DoParameterizedSQLCommand("INSERT INTO Accounts VALUES(@p1, @p2, 1000, 0)", usernameAndPasswordHash[0], usernameAndPasswordHash[1]);
                            Console.WriteLine(numRowsAffectedAddAccount + " row(s) affected");
                            DefaultDeckBuilder.AddDefaultDeckToPlayer(usernameAndPasswordHash[0]);
                            currentItem.sender.SendData(new byte[] { (byte)Protocol.UsernameNotTaken });
                        }
                        else
                        {
                            currentItem.sender.SendData(new byte[] { (byte)Protocol.UsernameTaken });
                        }
                    }
                    break;
                #endregion 
                #region CheckCredentials
                case Operation.CheckCredentials: //Checks if the given username and password are correct or if the player is already logged in. Returns the result of this. If the player is not logged in their decks, all the cards in the game and other details are sent to them.
                    {
                        string[] usernameAndPasswordHash = ((string)currentItem.data).Substring(1).Split('|');
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
                            Server.server.queue.Enqueue(new ActionItem(Operation.GetPlayerEloAndCoin, null, currentItem.sender));
                        }
                    }
                    break;
                #endregion
                #region AddToQueue
                case Operation.AddToQueue: //Adds the sender to the queue to play a game
                    {
                        int qs = Convert.ToInt32(((string)currentItem.data).Substring(1));
                        currentItem.sender.status = Status.InQueue;
                        currentItem.sender.queueStatus = qs;
                        currentItem.sender.queuetime = 0;
                    }
                    break;
                #endregion
                #region GetPlayerElo
                case Operation.GetPlayerEloAndCoin:
                    {
                        string username = currentItem.sender.userName;
                        object[][] data = Server.server.dbHandler.DoParameterizedSQLQuery("select elo, gold from accounts where username = @p1", username);
                        object[] o = data[0];
                        int elo = (int)o[0];
                        int coins = (int)o[1];
                        currentItem.sender.elo = elo;
                        currentItem.sender.coins = coins;
                        currentItem.sender.SendData(Server.addProtocolToArray(Server.toByteArray(elo + "a" + coins), Protocol.EloAndCoins));
                    }
                    break;
                #endregion
                #region TransmitDBData
                case Operation.TransmitDBData: //Transmits all of the cards, their effects and all the sender's deck to them. Finally sends to them that it is fine to enter the game.
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
                        currentItem.sender.SendData(new byte[] { (byte)Protocol.GoodCredentials });
                    }
                    break;
                #endregion
                #region CalculateEloCoinChanges
                case Operation.CalculateEloCoinChanges:
                    Client winner = currentItem.sender;
                    Client loser = Server.server.GetOpponent(winner);
                    int diff = winner.elo - loser.elo;
                    int g = 100 + (int)Math.Pow((double)diff / 60, 3d);
                    g = Math.Min(g, loser.elo);
                    winner.elo += g;
                    loser.elo -= g;
                    int winnerCoin = 35 + (int)(g * 1.2d);
                    int loserCoin = 15 + (int)(g * 0.8d);
                    Server.server.dbHandler.DoParameterizedSQLCommand("update accounts set elo = @p1, gold = @p2 where username = @p3", winner.elo, winnerCoin, winner.userName);
                    Server.server.dbHandler.DoParameterizedSQLCommand("update accounts set elo = @p1, gold = @p2 where username = @p3", loser.elo, loserCoin, loser.userName);
                    winner.SendData(Server.addProtocolToArray(Server.toByteArray(winner.elo + "a" + winnerCoin), Protocol.EloAndCoins));
                    loser.SendData(Server.addProtocolToArray(Server.toByteArray(loser.elo + "a" + loserCoin), Protocol.EloAndCoins));
                    break;
                #endregion
                #region BasicPack
                case Operation.BasicPack:
                    {
                        string[] cards = Server.server.GetPackCards(0.25, 0.15);
                        Server.server.UpdatePackCardsOnDB(currentItem.sender, cards);
                        TransmitObjectArray(cards, currentItem.sender, Protocol.PackCards);
                    }
                    break;
                #endregion
                #region PremiumPack
                case Operation.PremiumPack:
                    {
                        string[] cards = Server.server.GetPackCards(0.4, 0.3);
                        Server.server.UpdatePackCardsOnDB(currentItem.sender, cards);
                        TransmitObjectArray(cards, currentItem.sender, Protocol.PackCards);
                    }
                    break;
                #endregion
                case Operation.ClearDBDeckCards:
                    {
                        int deckid = Convert.ToInt32(currentItem.data);
                        if (deckid >= 0)
                            Server.server.dbHandler.DoParameterizedSQLCommand("delete from deckcards where deckid = @p1", deckid);
                    }
                    break;
                case Operation.AddCardToDeck:
                    {
                        string s = (string)currentItem.data;
                        string[] f = s.Split('|');
                        string cardName = f[1];
                        int deckid = Convert.ToInt32(f[0]);
                        int quantity = Convert.ToInt32(f[2]);
                        if (deckid < 0)
                        {
                            object[][] o = Server.server.dbHandler.DoParameterizedSQLQuery("select accountid from accounts where username = @p1", currentItem.sender.userName);
                            int accountid = Convert.ToInt32(o[0][0]);
                            Server.server.dbHandler.DoParameterizedSQLCommand("insert into decks values (@p1, 0)", accountid);
                            object[][] d = Server.server.dbHandler.DoParameterizedSQLQuery("select deckid from decks where accountid = @p1 and allcards = 0");
                            int x = -1;
                            foreach (object[] r in d)
                            {
                                x = Math.Max(x, Convert.ToInt32(r[0]));
                            }
                            currentItem.sender.SendData(Server.addProtocolToArray(new byte[] { (byte)x }, Protocol.NewDBDeckID));
                            deckid = x;
                        }
                        int cardID = (int)(Server.server.dbHandler.DoParameterizedSQLQuery("select cardid from cards where cardname = @p1", cardName)[0][0]);
                        Server.server.dbHandler.DoParameterizedSQLCommand("insert into deckcards values(@p1, @p2, @p3)", cardID, deckid, quantity);

                    }
                    break;
            }
        }
    }
}
