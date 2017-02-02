using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CourseworkClient.Gui
{
    class BigCard : GuiItem
    {
        public Card card;
        Vector2 loc;
        /// <summary>
        /// Create a BigCard with it being automatically placed for the InGameForm
        /// </summary>
        /// <param name="c">The card to create the BigCard from</param>
        public BigCard(Card c)
        {
            Primary.Log("Adding big card with parameter " + c);
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            int xPos = ((Primary.game.GraphicsDevice.Viewport.Width / 6) - Primary.game.cardOutlineBig.Width) / 2;
            loc = new Vector2(xPos, (Primary.game.GraphicsDevice.Viewport.Height - Primary.game.cardOutlineBig.Height) / 2);
            card = c;
            boundingBox = new Rectangle((int)loc.X, (int)loc.Y, Primary.game.cardOutlineBig.Width, Primary.game.cardOutlineBig.Height);
            subItems.Add(new CardText(new Rectangle(10 + (int)loc.X, 170 + (int)loc.Y, 176, 118), c.getEffectNames(), 12, c.getColourList()));
        }
        /// <summary>
        /// Create a BigCard at a designated position on the screen
        /// </summary>
        /// <param name="c">The card to create the BigCard from</param>
        /// <param name="e">The location of the BigCard</param>
        public BigCard(Card c, Vector2 e)
        {
            Primary.Log("Adding big card with parameters " + c + " | " + e);
            loc = e;
            card = c;
            boundingBox = new Rectangle((int)e.X, (int)e.Y, Primary.game.cardOutlineBig.Width, Primary.game.cardOutlineBig.Height);
            subItems.Add(new CardText(new Rectangle(10 + (int)e.X, 170 + (int)e.Y, 176, 118), c.getEffectNames(), 12, c.getColourList()));


        }

        public override void Draw(SpriteBatch sb)
        {
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            CardBuilder.DrawCard(card, loc, true, sb, Orientation.Up);
            foreach (GuiItem g in subItems) g.Draw(sb);
        }
        public override void Update()
        {
            foreach (GuiItem g in subItems) g.Update();
            //if (DeClicked()) ((InGameForm)Primary.game.currentForm).bigCard = null;

        }
        /// <summary>
        /// Change the location of the BigCard
        /// </summary>
        /// <param name="v">The new location of the BigCard</param>
        public void SetLocation(Vector2 v)
        {
            loc = v;
        }
    }
}
