using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    class CardText : ScrollableTextPane
    {
        const int charWidth = 5;
        const int charsPerLine = 30;
        public CardText(SpriteFont f, Rectangle rect, List<string> s, int n, int p, List<Color> c = null) : base(f, rect, s, n, p, c) { }
        public override void Draw(SpriteBatch sb)
        {
            Rectangle mp = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
            foreach (GuiItem g in subItems) g.Draw(sb);
            for (int i = position; i < position + numLines; i++)
            {
                if (i >= 0)
                {
                    int yPos = boundingBox.Y + (i * (pixelHeight + 2));
                    sb.DrawString(font, text[i], new Vector2(boundingBox.X, yPos), colours[i]);
                    if (mp.Intersects(new Rectangle(boundingBox.X, yPos, text[i].Length * charWidth, 10)))
                    {
                        Effect e = Effect.GetEffect(text[i]);
                        int numLines = 1 + (e.description.Length / charsPerLine);
                        sb.Draw(Primary.game.effectDescBox, new Rectangle(mp.X, mp.Y, 200, 30 * numLines), Color.White);
                        for (int x = 0; x < numLines; x++)
                        {
                            sb.DrawString(Primary.game.mainFont, e.description.Substring(x * charsPerLine, charsPerLine), new Vector2(mp.X + 5, 5 + mp.Y + (x * 30)), Color.Black);
                        }
                        sb.DrawString(Primary.game.mainFont, e.description.Substring(numLines * charsPerLine), new Vector2(mp.X + 5, mp.Y + (numLines * 30)), Color.Black);
                    }
                }
            }
        }
    }
}
