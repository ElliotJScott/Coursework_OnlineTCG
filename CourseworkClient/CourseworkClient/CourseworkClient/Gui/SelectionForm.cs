using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CourseworkClient.Gui
{
    class SelectionForm : Form
    {
        public InGameForm gameForm;
        public BigCard bigCard;
        public List<SmallCard> cards = new List<SmallCard>();
        public Function function;
        string text;
        /// <summary>
        /// Creates a new SelectionForm with a pre-existing SelectionItem
        /// </summary>
        /// <param name="form">The InGameForm that the user came from</param>
        /// <param name="s">The SelectionItem to select from</param>
        public SelectionForm(InGameForm form, SelectionItem s)
        {
            function = s.selection.function;
            gameForm = form;
            bigCard = null;
            PopulateCards(s);
            text = s.text;
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            formItems.Add(new sBackButton(new Rectangle( ((v.Width * 7)/8) - (Primary.game.buttonTexture.Width / 2), 200 + (v.Height - Primary.game.buttonTexture.Height) / 2, Primary.game.buttonTexture.Width, Primary.game.buttonTexture.Height ) ) );
            formItems.Add(new sSelectButton(new Rectangle(((v.Width * 7) / 8) - (Primary.game.buttonTexture.Width / 2), 210 + Primary.game.buttonTexture.Height + (v.Height - Primary.game.buttonTexture.Height) / 2, Primary.game.buttonTexture.Width, Primary.game.buttonTexture.Height)));

        }
        /// <summary>
        /// Creates a new SelectionForm with a list of cards and a function
        /// </summary>
        /// <param name="form">The InGameForm that the user came from</param>
        /// <param name="c">The list of cards to select from</param>
        /// <param name="f">The function to perform on the selection</param>
        public SelectionForm(InGameForm form, List<SmallCard> c, Function f)
        {
            function = f;
            gameForm = form;
            bigCard = null;
            AddSmallCards(c);
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            formItems.Add(new sBackButton(new Rectangle(((v.Width * 7) / 8) - (Primary.game.buttonTexture.Width / 2), 200 + (v.Height - Primary.game.buttonTexture.Height) / 2, Primary.game.buttonTexture.Width, Primary.game.buttonTexture.Height)));
            formItems.Add(new sSelectButton(new Rectangle(((v.Width * 7) / 8) - (Primary.game.buttonTexture.Width / 2), 210 + Primary.game.buttonTexture.Height + (v.Height - Primary.game.buttonTexture.Height) / 2, Primary.game.buttonTexture.Width, Primary.game.buttonTexture.Height)));

        }
        /// <summary>
        /// Populates the form with the cards from the SelectionItem given
        /// </summary>
        /// <param name="item">The SelectionItem passed to the form</param>
        void PopulateCards(SelectionItem item)
        {
            cards.Clear();
            List<SmallCard> selectionResult = item.selection.GetCards();
            AddSmallCards(selectionResult);
        }
        /// <summary>
        /// Populates the form with the list of cards given
        /// </summary>
        /// <param name="f">The list of cards to populate the form with</param>
        void AddSmallCards(List<SmallCard> f)
        {
            int cardAreaWidth = (Primary.game.GraphicsDevice.Viewport.Width * 3) / 4;
            int numCardsAcross = cardAreaWidth / Primary.game.cardOutlineSmall.Width;
            for (int i = 0; i < f.Count; i++)
            {
                int cardX = Primary.game.cardOutlineSmall.Width * (i % numCardsAcross);
                int cardY = Primary.game.cardOutlineSmall.Height * (i / numCardsAcross);
                SmallCard c = f[i].CloneWithoutReferenceForSelection();
                c.UpdateLocation(new Vector2(cardX, cardY));
                cards.Add(c);
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            foreach (SmallCard c in cards)
            {
                if (!c.drawnBig)
                    c.Draw(sb);
            }
            if (bigCard != null)
                bigCard.Draw(sb);
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            if (text != null)
                sb.DrawString(Primary.game.mainFont, text, new Vector2(((v.Width * 7) / 8) - (Primary.game.buttonTexture.Width / 2), 220 + (2 * Primary.game.buttonTexture.Height) + (v.Height - Primary.game.buttonTexture.Height) / 2), Color.White);
        }
        public override void Update()
        {
            foreach (SmallCard c in cards) c.Update();
            bigCard?.Update();
            base.Update();
        }
        /// <summary>
        /// Gets the currently selected card
        /// </summary>
        /// <returns>The currently selected card</returns>
        internal SmallCard GetSelectedCard()
        {
            foreach (SmallCard c in cards)
            {
                if (c.drawnBig) return c;
            }
            throw new InvalidOperationException();
        }
    }
}
