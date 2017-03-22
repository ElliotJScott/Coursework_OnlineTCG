using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    class DeckCardItem : GuiItem
    {
        Card card;
        int num;
        Vector2 pos;
        bool inAllCards;
        public DeckCardItem(Card c, int n, Vector2 p, bool b)
        {
            card = c;
            num = n;
            pos = p;
            inAllCards = b;
        }
        public override void Draw(SpriteBatch sb)
        {
            CardBuilder.DrawCard(card, pos, true, sb, true, false);
            Texture2D tex = changethislater;
            sb.Draw(tex,pos - new Vector2(tex.Width / 2, tex.Height / 2), Color.White);
            Vector2 v = Primary.game.mainFont.MeasureString(num.ToString());
            sb.DrawString(Primary.game.mainFont, num.ToString(), pos - (v / 2), Color.Black);
        }

        public override void Update()
        {
            DeckManagerForm currentForm = (DeckManagerForm)Primary.game.currentForm;
            if (Clicked() && num > 0)
            {
                num--;
                if (inAllCards)
                {
                    currentForm.decks[currentForm.currentDeck].AddAdditionalCard(card);

                }
            }
            if (inAllCards)
            {
                num = Deck.allOwnedCards.GetCardQuantity(card) - currentForm.decks[currentForm.currentDeck].GetCardQuantity(card);
            }
        }
    }
}
