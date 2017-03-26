using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    /// <summary>
    /// Defines a card in a deck and the quantity in it
    /// </summary>
    public class DeckItem : IComparable
    {
        public Card card;
        public int quantity;
        public DeckItem(Card c, int q)
        {
            card = c;
            quantity = q;
        }
        /// <summary>
        /// Compares two deckitems for sorting purposes
        /// </summary>
        /// <param name="obj">The deckitem to compare to</param>
        /// <returns>Negative if this should come first, 0 if they are equal, positive if the parameter should come first</returns>
        public int CompareTo(object obj)
        {
            if (obj.GetType() != GetType()) throw new ArgumentException();
            DeckItem d = (DeckItem)obj;
            if (card.type == CardType.Upgrade)
            {
                if (d.card.type != CardType.Upgrade) return 1;
                else return card.name.CompareTo(d.card.name);
            }
            if (card.type == CardType.Tech)
            {
                switch (d.card.type)
                {
                    case CardType.Upgrade:
                        return -1;
                    case CardType.Tech:
                        return card.name.CompareTo(d.card.name);
                    case CardType.Unit:
                        return 1;
                }
            }
            if (card.type == CardType.Unit)
            {
                if (d.card.type != CardType.Unit) return -1;
                else
                {
                    foreach (string s in Gui.InGameForm.races)
                    {
                        if (card.hasEffect(s) && d.card.hasEffect(s)) return card.name.CompareTo(d.card.name);
                        else if (card.hasEffect(s)) return -1;
                        else if (d.card.hasEffect(s)) return 1;
                    }

                }
            }
            return card.name.CompareTo(d.card.name);
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
        /// <summary>
        /// Creates a new deck with an id
        /// </summary>
        /// <param name="id">The primary key of the deck in the database</param>
        public Deck(int id = -1)
        {
            dbID = id;
        }
        /// <summary>
        /// Creates a new deck with an id and the cards in the deck
        /// </summary>
        /// <param name="id">The primary key of the deck in the database</param>
        /// <param name="m">The cards in the main deck</param>
        /// <param name="u">The cards in the upgrade deck</param>
        public Deck(int id, List<DeckItem> m, List<DeckItem> u)
        {
            dbID = id;
            foreach (DeckItem d in m) mainDeck.Add(d);
            foreach (DeckItem d in u) upgrades.Add(d);
        }
        public int GetDeckCount(bool md)
        {
            int x = 0;
            List<DeckItem> d = md ? mainDeck : upgrades;
            foreach (DeckItem r in d) x += r.quantity;
            return x;
        }
        public void DecreaseQuantity(Card c, int x)
        {
            foreach (List<DeckItem> f in new List<DeckItem>[] { mainDeck, upgrades })
            {
                foreach (DeckItem d in f)
                {
                    if (d.card == c)
                    {
                        d.quantity -= x;
                        if (d.quantity <= 0)
                        {
                            f.Remove(d);
                           
                        }
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// Adds a card to a pre-existing deck
        /// </summary>
        /// <param name="c">The card to add</param>
        /// <param name="deckID">The id of the deck to add it to</param>
        /// <param name="quantity">The number of the card to add</param>
        public static void AddCardToDeck(Card c, int deckID, int quantity = 1)
        {
            foreach (Deck d in decks)
            {
                AddToDeck(d, c, deckID, quantity);                
            }
            AddToDeck(allOwnedCards, c, deckID, quantity);
        }
        /// <summary>
        /// Adds a given card to a given deck
        /// </summary>
        /// <param name="d">The deck to add it to</param>
        /// <param name="c">The card to add</param>
        /// <param name="deckID">The id of the intended deck</param>
        /// <param name="quantity">The quantity of the card to add</param>
        public static void AddToDeck(Deck d, Card c, int deckID, int quantity)
        {
            if (d.dbID == deckID)
            {
                if (c.type == CardType.Upgrade)
                    d.upgrades.Add(new DeckItem(c, quantity));
                else d.mainDeck.Add(new DeckItem(c, quantity));

            }
        }
        public int GetNumWithEffect(Effect e)
        {
            int x = 0;
            foreach (List<DeckItem> f in new List<DeckItem>[] { mainDeck, upgrades })
                foreach (DeckItem d in f)
                    if (d.card.effects.Contains(e))
                        x++;
            return x;
        }
        public void AddAdditionalCard(Card c)
        {
            foreach (List<DeckItem> d in new List<DeckItem>[] { mainDeck, upgrades })
            {
                foreach (DeckItem f in d)
                {
                    if (f.card == c)
                    {
                        f.quantity++;
                        return;
                    }
                }
                if (c.type != CardType.Upgrade)
                {
                    mainDeck.Add(new DeckItem(c, 1));
                    return;
                }
            }
            upgrades.Add(new DeckItem(c, 1));
        }

        public int GetCardQuantity(Card c)
        {
            foreach (List<DeckItem> f in new List<DeckItem>[] { mainDeck, upgrades })
            {
                foreach (DeckItem d in f)
                {
                    if (d.card == c) return d.quantity;
                }
            }
            return 0;
        }

    }

}
