using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    public class Form
    {
        Texture2D background;
        List<GuiItem> formItems = new List<GuiItem>();

        public Form(Texture2D tex)
        {
            background = tex;
        }
        public void Update()
        {
            foreach (GuiItem g in formItems) g.Update();
        }
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(background, new Rectangle(0, 0, background.Width, background.Height), Color.White);
        }
    }
}
