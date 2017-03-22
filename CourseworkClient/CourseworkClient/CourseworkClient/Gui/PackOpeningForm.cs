using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    class PackOpeningForm : Form
    {
        public PackOpeningForm(Card[] cards)
        {
            background = Primary.game.mainMenuBackground;
            for (int i = 0; i < cards.Length; i++)
            {
                formItems.Add(new BigCard(cards[i], new Vector2(120 * (i + 1), 50)));
            }
        }
        public override void Update()
        {
            base.Update();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }
    }
}
