using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CourseworkClient.Gui
{
    class ScrollableTextPane : GuiItem
    {
        public List<string> text = new List<string>();
        public int numLines;
        public int position;
        public int pixelHeight;
        public SpriteFont font;
        public List<Color> colours;
        public ScrollableTextPane(SpriteFont f, Rectangle rect, List<string> s, int n, int p, List<Color> c = null)
        {
            font = f;
            boundingBox = rect;
            text = s;
            numLines = n;
            pixelHeight = p;
            position = s.Count - n;
            if (c == null) colours = fillColourArray(Color.Black, s.Count);
            else colours = c;
            subItems.Add(new ScrollArrow(new Rectangle(rect.X + rect.Width - Primary.game.greenArrowTexture.Width, rect.Y, Primary.game.greenArrowTexture.Width, Primary.game.greenArrowTexture.Height), Orientation.Up));
            subItems.Add(new ScrollArrow(new Rectangle(rect.X + rect.Width - Primary.game.greenArrowTexture.Width, rect.Y + rect.Height - Primary.game.greenArrowTexture.Height, Primary.game.greenArrowTexture.Width, Primary.game.greenArrowTexture.Height), Orientation.Down));
        }
        public override void Draw(SpriteBatch sb)
        {
            foreach (GuiItem g in subItems) g.Draw(sb);
            for (int i = position; i < position + numLines; i++)
            {
                if (i >= 0)
                {
                    sb.DrawString(font, text[i], new Vector2(boundingBox.X, boundingBox.Y + (i * (pixelHeight + 2))), colours[i]);
                }
            }
        }
        public List<Color> fillColourArray(Color c, int l)
        {
            List<Color> output = new List<Color>();
            for (int i = 0; i < l; i++) output.Add(c);
            return output;
        }
        public override void Update()
        {

            bool[] b = canScroll();
            for (int i = 0; i < 2; i++)
            {
                if (subItems[i].Clicked(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)) && b[i])
                {
                    position += i + ((i % 2) - 1);
                }
            }

        }
        public bool[] canScroll()
        {
            if (text.Count <= numLines) return new bool[]{ false, false };
            bool up = false;
            bool down = false;
            if (position > 0) up = true;
            if (position < text.Count - numLines) down = true;
            return new bool[] { up, down };
        }
        public void addLine(string s, Color? c)
        {
            text.Add(s);
            position++;
            if (!c.HasValue) colours.Add(Color.Black);
            else colours.Add(c.Value);
        }
    }
}
