using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    class DeckItem
    {
        Card card;
        int quantity;
        public DeckItem(Card c, int q)
        {
            card = c;
            quantity = q;
        }

    }
    class Deck
    {
        List<DeckItem> allOwnedCards = new List<DeckItem>();
        List<DeckItem>[] decks = new List<DeckItem>[9];
    }
}
