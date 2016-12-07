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
            //formItems.Add(new )
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
