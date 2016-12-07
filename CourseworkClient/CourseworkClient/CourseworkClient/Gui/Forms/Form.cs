using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    public abstract class Form
    {
        public Texture2D background;
        public List<GuiItem> formItems = new List<GuiItem>();

        public virtual void Update()
        {
            foreach (GuiItem g in formItems) g.Update();
        }
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(background, new Rectangle(0, 0, Primary.game.GraphicsDevice.Viewport.Width, Primary.game.GraphicsDevice.Viewport.Height), Color.White);
            foreach (GuiItem g in formItems) g.Draw(sb);
            sb.DrawString(Primary.game.mainFont, Primary.game.connected ? "Connected" : "Not connected : " + Primary.game.connectTimer, new Vector2(0, Primary.game.GraphicsDevice.Viewport.Height - 20), Color.Black);
        }
        public abstract int GetFormID();
    }
}
