using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    public struct DeckItem
    {
        public Card card;
        public int quantity;
        public DeckItem(Card c, int q)
        {
            card = c;
            quantity = q;
        }

    }
    public class Deck
    {
        
        public int dbID;
        public static Deck allOwnedCards;
        public static List<Deck> decks = new List<Deck>();
        public List<DeckItem> mainDeck = new List<DeckItem>();
        public List<DeckItem> upgrades = new List<DeckItem>();
        const int maxDecks = 9;
        const int maxCards = 40;
        const int maxUpgrades = 10;
        const int minUpgrades = 3;
        public Deck(int id)
        {
            dbID = id;
        }
        public Deck(int id, List<DeckItem> m, List<DeckItem> u)
        {
            dbID = id;
            foreach (DeckItem d in m) mainDeck.Add(d);
            foreach (DeckItem d in u) upgrades.Add(d);
        }
        public static void AddCardToDeck(Card c, int deckID, int quantity = 1)
        {
            foreach (Deck d in decks)
            {
                AddToDeck(d, c, deckID, quantity);                
            }
            AddToDeck(allOwnedCards, c, deckID, quantity);
        }
        public static void AddToDeck(Deck d, Card c, int deckID, int quantity)
        {
            if (d.dbID == deckID)
            {
                if (c.type == CardType.Upgrade)
                    d.upgrades.Add(new DeckItem(c, quantity));
                else d.mainDeck.Add(new DeckItem(c, quantity));

            }
        }
    }
}
