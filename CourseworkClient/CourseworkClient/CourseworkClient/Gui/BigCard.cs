using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CourseworkClient.Gui
{
    enum Location
    {
        InHand,
        InPlay,
    }
    class BigCard : GuiItem
    {
        Card card;
        public BigCard(Card c)
        {
            card = c;
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            subItems.Add(new CardText(Primary.game.mainFont, new Rectangle(10 + (v.Width - Primary.game.cardOutlineBig.Width) / 2, 100 + (v.Height - Primary.game.cardOutlineBig.Height) / 2, 100, 100), c.getEffectNames(), 10, 10, c.getColourList()));
        }
        public override void Draw(SpriteBatch sb)
        {
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            CardBuilder.DrawCard(card, new Vector2((v.Width - Primary.game.cardOutlineBig.Width) / 2, (v.Height - Primary.game.cardOutlineBig.Height) / 2), true, sb);
            foreach (GuiItem g in subItems) g.Draw(sb);
        }

        public override void Update()
        {
            MouseState m = Mouse.GetState();
            Vector2 mp = new Vector2(m.X, m.Y);
            if (Clicked(mp) || DeClicked(mp))
            {
                foreach (GuiItem g in Primary.game.currentForm.formItems)
                {
                    if (g.GetType() == typeof(SmallCard))
                    {
                        ((SmallCard)g).drawnBig = false;
                    }
                }
            }
        }
    }
}
