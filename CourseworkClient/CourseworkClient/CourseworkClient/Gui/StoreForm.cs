using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    class StoreForm : Form
    {
        public StoreForm()
        {
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            formItems.Add(new FormChangeButton(new Rectangle(0, 0, 100, 50), "Main Menu", FormChangeButtonTypes.StoreToMainMenu));
            formItems.Add(new BasicPackButton(new Rectangle((v.Width / 2) - 50, (v.Height / 2) - 25, 100, 50), "Basic Pack"));
            formItems.Add(new PremiumPackButton(new Rectangle((v.Width / 2) - 50, (v.Height / 2) + 35, 100, 50), "Premium Pack"));
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
