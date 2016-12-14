using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
    struct DeckItem
    {
        public int number;
        public string name;
        public DeckItem(int n, string s)
        {
            number = n;
            name = s;
        }
    }
    class DefaultDeckBuilder
    {
        public static DeckItem[] ultramarineDefaultDeck =
            {
            new DeckItem(1, "Captain Titus"),
            new DeckItem(1, "Rhino"),
            new DeckItem(1, "Dreadnought"),
            new DeckItem(2, "Terminator Squad"),
            new DeckItem(2, "Devastator Squad"),
            new DeckItem(2, "Assault Squad"),
            new DeckItem(3, "Biker Squad"),
            new DeckItem(4, "Space Marines"),
            new DeckItem(1, "Pot of Greed"),
            new DeckItem(2, "Tech Jammer"),
            new DeckItem(1, "Ambush"),
            new DeckItem(2, "Repair Pack"),
            new DeckItem(1, "Propaganda"),
            new DeckItem(2, "Industrial Investment"),
            new DeckItem(2, "Power Extraction"),
            new DeckItem(1, "Death in Honour"),
            new DeckItem(2, "Repair and Recover"),
            new DeckItem(2, "Terminator Armour"),
            new DeckItem(1, "Psyker: Telekinesis")
        };
        public static DeckItem[] tyranidDefaultDeck =
            {
            new DeckItem(1, "Hive Tyrant"),
            new DeckItem(1, "Tyrant Guard"),
            new DeckItem(1, "Carnifex"),
            new DeckItem(1, "Tyranid Warrior"),
            new DeckItem(2, "Zoanthropes"),
            new DeckItem(2, "Genestealer Brood"),
            new DeckItem(4, "Termagant Brood"),
            new DeckItem(4, "Hormagaunt Brood"),
            new DeckItem(1, "Pot of Greed"),
            new DeckItem(2, "Tech Jammer"),
            new DeckItem(2, "Ambush"),
            new DeckItem(2, "Stimpack"),
            new DeckItem(1, "Equivalent Exchange"),
            new DeckItem(2, "Demilitarisation"),
            new DeckItem(2, "Two-Pronged Attack"),
            new DeckItem(1, "Anti-Vehicle Artillery"),
            new DeckItem(1, "Sabotage"),
            new DeckItem(2, "Bonesword"),
            new DeckItem(1, "Psyker: Leech Essence")
        };
        public static void AddDefaultDeckToPlayer(string u)
        {
            int f = Server.server.rng.Next(2);
            DeckItem[] deck = f == 0 ? ultramarineDefaultDeck : tyranidDefaultDeck;
            int accountID = (int)(Server.server.dbHandler.DoParameterizedSQLQuery("select accountid from accounts where username = @p1", u)[0][0]);
            int[] deckIDs = new int[2];
            for (int i = 0; i < 2; i++)
            {
                Server.server.dbHandler.DoParameterizedSQLCommand("insert into decks values(@p1, @p2)", accountID, i);
                deckIDs[i] =(int)(Server.server.dbHandler.DoParameterizedSQLQuery("select deckid from decks where accountid = @p1 and allcards = @p2", accountID, i)[0][0]);
            }
            foreach (DeckItem d in deck)
            {
                for (int i = 0; i < 2; i++)
                {
                    int cardID = (int)(Server.server.dbHandler.DoParameterizedSQLQuery(" select cardid from cards where cardname = @p1", d.name)[0][0]);
                    Server.server.dbHandler.DoParameterizedSQLCommand("insert into deckcards values(@p1, @p2, @p3)", cardID, deckIDs[i], d.number);
                }
            }
        }
    }
}
