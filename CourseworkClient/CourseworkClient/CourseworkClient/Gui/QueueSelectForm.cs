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
        /// <summary>
        /// Creates a new QueueSelectForm
        /// </summary>
        public QueueSelectForm()
        {
            background = Primary.game.mainMenuBackground;
            formItems.Add(new FormChangeButton(new Rectangle(2, 2, 100, 50), "Back", FormChangeButtonTypes.OptionsToMainMenu));
            formItems.Add(new AddToQueueButton(new Rectangle(2, 100, 100, 50), "Quick Game", 0));
            formItems.Add(new AddToQueueButton(new Rectangle(2, 200, 100, 50), "Competitive Game", 1));
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
