using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    public class QueueForm : Form
    {
        public int timeInQueue;
        /// <summary>
        /// Creates a new QueueSelectForm
        /// </summary>
        public QueueForm()
        {
            timeInQueue = 0;
            background = Primary.game.mainMenuBackground;
            formItems.Add(new FormChangeButton(new Rectangle(0, 0, 200, 50), "Leave Queue", FormChangeButtonTypes.QueueToMainMenu));
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.DrawString(Primary.game.mainFont, "Time in queue : " + timeInQueue, new Vector2(200, 200), Color.White);
        }

    }
}
