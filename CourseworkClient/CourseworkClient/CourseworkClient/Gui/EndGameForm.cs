using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    class EndGameForm : Form
    {
        /// <summary>
        /// Creates a new EndGameForm
        /// </summary>
        public EndGameForm()
        {
            formItems.Add(new FormChangeButton(new Rectangle(100, 100, 150, 50), "Main Menu", FormChangeButtonTypes.EndGameToMainMenu));
            background = Primary.game.mainMenuBackground;
        }

        public override void Update()
        {
            base.Update();
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.DrawString(Primary.game.mainFont, string.Format("Your ({0}) elo - {1}\nYour coins - {2}", Primary.game.username, Primary.game.elo, Primary.game.coins), new Vector2(200, 200), Color.White);
        }
    }
}
