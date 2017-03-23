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
        List<BigCard> cards = new List<BigCard>();
        public PackOpeningForm(Card[] c)
        {
            background = Primary.game.mainMenuBackground;
            for (int i = 0; i < c.Length; i++)
            {
                cards.Add(new BigCard(c[i], new Vector2(220 * i, 60)));
            }
            formItems.Add(new FormChangeButton(new Rectangle(0, 0, 100, 50), "Main Menu", FormChangeButtonTypes.StoreToMainMenu));
        }
        public override void Update()
        {
            base.Update();
            for (int i = cards.Count - 1; i >= 0; i--) cards[i].Update();
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            for (int i = cards.Count - 1; i >= 0; i--) cards[i].Draw(sb);
        }
    }
}
