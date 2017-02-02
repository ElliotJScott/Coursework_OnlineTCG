using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    /// <summary>
    /// Defines a card in a deck and the quantity in it
    /// </summary>
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
        /// <summary>
        /// Creates a new deck with an id
        /// </summary>
        /// <param name="id">The primary key of the deck in the database</param>
        public Deck(int id)
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
    }
}
