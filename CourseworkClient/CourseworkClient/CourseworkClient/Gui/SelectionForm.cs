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
        public SelectionForm(InGameForm form, SelectionItem s)
        {
            function = s.selection.function;
            gameForm = form;
            bigCard = null;
            PopulateCards(s);
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            formItems.Add(new sBackButton(new Rectangle( ((v.Width * 7)/8) - (Primary.game.buttonTexture.Width / 2), 200 + (v.Height - Primary.game.buttonTexture.Height) / 2, Primary.game.buttonTexture.Width, Primary.game.buttonTexture.Height ) ) );
            formItems.Add(new sSelectButton(new Rectangle(((v.Width * 7) / 8) - (Primary.game.buttonTexture.Width / 2), 210 + Primary.game.buttonTexture.Height + (v.Height - Primary.game.buttonTexture.Height) / 2, Primary.game.buttonTexture.Width, Primary.game.buttonTexture.Height)));

        }
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
        void PopulateCards(SelectionItem item)
        {
            cards.Clear();
            List<SmallCard> selectionResult = item.selection.GetCards();
            AddSmallCards(selectionResult);
        }
        void AddSmallCards(List<SmallCard> cards)
        {
            int cardAreaWidth = (Primary.game.GraphicsDevice.Viewport.Width * 3) / 4;
            int numCardsAcross = cardAreaWidth / Primary.game.cardOutlineSmall.Width;
            for (int i = 0; i < cards.Count; i++)
            {
                int cardX = Primary.game.cardOutlineSmall.Width * (i % numCardsAcross);
                int cardY = Primary.game.cardOutlineSmall.Height * (i / numCardsAcross);
                SmallCard c = cards[i].CloneWithoutReferenceForSelection();
                c.UpdateLocation(new Vector2(cardX, cardY));
                cards.Add(c);
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            foreach (SmallCard c in cards)
            {
                c.Draw(sb);
            }
            if (bigCard != null) CardBuilder.DrawCard(bigCard.card, new Vector2(200, 200), true, sb, Orientation.Up);
            base.Draw(sb);
        }
        public override void Update()
        {
            foreach (SmallCard c in cards) c.Update();
            bigCard?.Update();
            base.Update();
        }

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
