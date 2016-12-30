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
        public bool locked = false;
        string lockMessage = "";
        public int bufferAnimTimer = 0;
        public const int numBufferFrames = 8;
        public virtual void Update()
        {
            if (!locked) 
                for (int i = 0; i < formItems.Count; i++) formItems[i].Update();
        }
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(background == null ? Primary.game.mainMenuBackground : background, new Rectangle(0, 0, Primary.game.GraphicsDevice.Viewport.Width, Primary.game.GraphicsDevice.Viewport.Height), Color.White);
            foreach (GuiItem g in formItems) g.Draw(sb);
       
            if (bufferAnimTimer++ >= (numBufferFrames * 10 - 1)) bufferAnimTimer = 0;
        }
        public void PostDraw(SpriteBatch sb)
        {

            if (locked)
            {
                Viewport v = Primary.game.GraphicsDevice.Viewport;
                sb.Draw(Primary.game.screenDarkener, new Rectangle(0, 0, v.Width, v.Height), Color.White);
                sb.Draw(Primary.game.lockMessageBox, new Rectangle((v.Width / 2) - 200, (v.Height / 2) - 200, 400, 400), Color.White);
                sb.Draw(Primary.game.loadingIcon, new Rectangle((v.Width / 2) - 75, (v.Height / 2) - 190, 150, 150), new Rectangle(0, (bufferAnimTimer/10) * 150, 150, 150), Color.White);
                sb.DrawString(Primary.game.mainFont, lockMessage, new Vector2((v.Width / 2) - 190, v.Height / 2), Color.Red);
            }
        }
        public void Lock(string s)
        {
            locked = true;
            lockMessage = s;
        }
        public void Unlock()
        {
            locked = false;
        }
    }
}
