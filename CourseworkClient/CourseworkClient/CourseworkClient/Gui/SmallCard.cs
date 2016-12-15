using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CourseworkClient.Gui
{
    class SmallCard : GuiItem
    {
        Card card;
        Vector2 loc;
        public bool drawnBig;
        bool hidden;
        public SmallCard(Card c, Vector2 l, bool h = false)
        {
            card = c;
            loc = l;
            hidden = h;
        }
        public override void Draw(SpriteBatch sb)
        {
            if (!drawnBig)
                CardBuilder.DrawCard(card, loc, false, sb);
        }
       
        public override void Update()
        {
            if (!drawnBig) {
                MouseState m = Mouse.GetState();
                if (Clicked(new Vector2(m.X, m.Y)))
                {
                    drawnBig = true;
                    Primary.game.currentForm.formItems.Add(new BigCard(card));
                }
            }
            //Update this
        }
    }
}
