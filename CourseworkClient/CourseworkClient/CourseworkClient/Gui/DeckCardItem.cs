using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    /// <summary>
    /// These are the items in the deck manager form
    /// </summary>
    class DeckCardItem : GuiItem
    {
        BigCard card;
        int num;
        Vector2 pos;
        bool inAllCards;
        /// <summary>
        /// Creates a new DeckCardItem
        /// </summary>
        /// <param name="c">The card to draw</param>
        /// <param name="p">The location to draw this at</param>
        /// <param name="b">Whether it is in all cards or in a deck</param>
        public DeckCardItem(Card c, Vector2 p, bool b)
        {
            DeckManagerForm currentForm = (DeckManagerForm)Primary.game.currentForm;
            pos = p;
            card = new BigCard(c, pos);
            if (inAllCards) num = Deck.allOwnedCards.GetCardQuantity(c) - currentForm.decks[currentForm.currentDeck].GetCardQuantity(c);
            else num = currentForm.decks[currentForm.currentDeck].GetCardQuantity(c);
            inAllCards = b;
            boundingBox = card.boundingBox;
        }
        public override void Draw(SpriteBatch sb)
        {
            card.Draw(sb);
            Texture2D tex = Primary.game.cardCountCircle;
            sb.Draw(tex,pos - new Vector2(tex.Width / 2, tex.Height / 2), Color.White);
            Vector2 v = Primary.game.mainFont.MeasureString(num.ToString());
            sb.DrawString(Primary.game.mainFont, num.ToString(), pos - (v / 2), Color.Black);
        }

        public override void Update()
        {
            try {
                card.Update();
                DeckManagerForm currentForm = (DeckManagerForm)Primary.game.currentForm;
                if (Clicked() && num > 0)
                {
                    if (inAllCards)
                    {
                        int f = currentForm.decks[currentForm.currentDeck].GetCardQuantity(card.card);
                        bool b = true;
                        if (f >= 4) b = false;
                        if (f >= 2 && card.card.hasEffect("Semi-Unique")) b = false;
                        if (f >= 1 && card.card.hasEffect("Unique")) b = false;
                        if (currentForm.decks[currentForm.currentDeck].GetNumWithEffect(Effect.GetEffect("Leader")) >= 2 && card.card.hasEffect("Leader")) b = false;
                        if (b)
                        {
                            currentForm.decks[currentForm.currentDeck].AddAdditionalCard(card.card);
                        }
                        num = Deck.allOwnedCards.GetCardQuantity(card.card) - currentForm.decks[currentForm.currentDeck].GetCardQuantity(card.card);
                    }
                    else
                    {
                        num--;
                        currentForm.decks[currentForm.currentDeck].DecreaseQuantity(card.card, 1);
                    }
                    currentForm.UpdateDeckCardItems();
                    return;
                }
                if (inAllCards)
                {
                    num = Deck.allOwnedCards.GetCardQuantity(card.card) - currentForm.decks[currentForm.currentDeck].GetCardQuantity(card.card);
                }
            }
            catch { }
        }
    }
}
