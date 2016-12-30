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
        InGameForm gameForm;
        List<Selection> selection;
        public BigCard bigCard;
        public List<SmallCard> cards = new List<SmallCard>();
        public SelectionForm(InGameForm form, Selection[] s)
        {
            gameForm = form;
            selection = s.ToList();
            bigCard = null;
            PopulateCards();
        }
        void PopulateCards()
        {
            cards.Clear();
            Selection currentSelection = selection[selection.Count - 1];
            List<SmallCard> selectionResult = currentSelection.GetCards();
            int cardAreaWidth = (Primary.game.GraphicsDevice.Viewport.Width * 3) / 4;
            int numCardsAcross = cardAreaWidth / Primary.game.cardOutlineSmall.Width;
            for (int i = 0; i < selectionResult.Count; i++)
            {
                int cardX = Primary.game.cardOutlineSmall.Width * (i % numCardsAcross);
                int cardY = Primary.game.cardOutlineSmall.Height * (i / numCardsAcross);
                SmallCard c = selectionResult[i].CloneWithoutReferenceForSelection();
                c.UpdateLocation(new Vector2(cardX, cardY));
                cards.Add(c);
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
        public override void Update()
        {
            base.Update();
        }
    }
}
