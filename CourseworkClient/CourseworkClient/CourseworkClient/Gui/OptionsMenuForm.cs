using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    class OptionsMenuForm : Form
    {
        public OptionsMenuForm()
        {
            background = Primary.game.loginScreenBackground;
            formItems.Add(new FormChangeButton(new Rectangle(2, 2, 100, 50), "Back", FormChangeButtonTypes.OptionsToMainMenu));
            for (int i = 1; i < 32; i += 2)
            {
                int y = i / 8;
                int x = i % 8;
                formItems.Add(new BigCard(Card.allCards[Primary.game.rng.Next(Card.allCards.Count)], new Vector2(x * Primary.game.cardOutlineBig.Width, y * Primary.game.cardOutlineBig.Height)));
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
