using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    public class QueueSelectForm : Form
    {
        public QueueSelectForm()
        {
            background = Primary.game.loginScreenBackground;
            formItems.Add(new FormChangeButton(new Rectangle(2, 2, 100, 50), "Back", FormChangeButtonTypes.OptionsToMainMenu));
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
