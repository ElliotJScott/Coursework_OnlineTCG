using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CourseworkClient.Gui
{
    class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            background = Primary.game.loginScreenBackground;
            formItems.Add(new FormChangeButton(new Microsoft.Xna.Framework.Rectangle(2, 2, 100, 50), "Options", FormChangeButtonTypes.MainMenuToOptions));
        }

        public override int GetFormID() => 2;
        
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
