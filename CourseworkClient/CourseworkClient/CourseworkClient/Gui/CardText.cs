using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    /// <summary>
    /// Used to draw the effects on a big card and allows for them to be hovered over to examine their effects. Also allows for them to be scrolled through.
    /// </summary>
    class CardText : ScrollableTextPane
    {
        const int charWidth = 9;
        const int charsPerLine = 30;
        const int maxLines = 8;
        /// <summary>
        /// Initialises a CardText item in a BigCard.
        /// </summary>
        /// <param name="rect">The bounding box of the </param>
        /// <param name="s">The lines to draw</param>
        /// <param name="p">Has no effect for a CardText item</param>
        /// <param name="c">The colours to draw each line at</param>
        public CardText(Rectangle rect, List<string> s, int p, List<Color> c = null) : base(rect, s, 14358, p, c)
        {
            position = 0;
            numLines = s.Count < maxLines ? s.Count : maxLines;
        }
        public override void Draw(SpriteBatch sb)
        {
            Rectangle mp = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
            foreach (GuiItem g in subItems) g.Draw(sb);
            string highlightedDesc = "";
            for (int i = position; i < position + numLines; i++)
            {

                if (i >= 0)
                {
                    int yPos = boundingBox.Y + ((i-position) * pixelHeight);
                    sb.DrawString(Primary.game.cardTextFont, text[i].Length > 21? text[i].Substring(0, 18) + "..." : text[i], new Vector2(boundingBox.X + 4, yPos), colours[i]);
                    if (mp.Intersects(new Rectangle(boundingBox.X, yPos, text[i].Length * charWidth, pixelHeight)))
                    {

                        highlightedDesc = Effect.GetEffect(text[i]).name + " - " + Effect.GetEffect(text[i]).description;
                    }
                }
            }
            if (highlightedDesc != "")
            {

                string[] effectDescText = stringToLines(charsPerLine, highlightedDesc);
                sb.Draw(Primary.game.effectDescBox, new Rectangle(mp.X, mp.Y, -10 + (charWidth * charsPerLine), 15 + (pixelHeight * effectDescText.Length)), Color.White);
                for (int x = 0; x < effectDescText.Length; x++)
                {
                    sb.DrawString(Primary.game.cardTextFont, effectDescText[x], new Vector2(mp.X + 5, mp.Y + (x * pixelHeight)), Color.Black);
                }
            }
           
        }
        /// <summary>
        /// Turns a piece of text into a number of lines.
        /// </summary>
        /// <param name="numChars">The maximum number of characters per line</param>
        /// <param name="s">The input text</param>
        /// <returns>A string array containing the lines</returns>
        public static string[] stringToLines(int numChars, string s)
        {
            string[] splitted = s.Split(' ');
            List<string> output = new List<string>();
            string tempString = "";
            for (int i = 0; i < splitted.Length; i++)
            {
                if (!(splitted[i].Length + 1 + tempString.Length <= numChars))
                {
                    output.Add(tempString);
                    tempString = "";
                }
                tempString = (tempString + " " + splitted[i]).Trim();
            }
            if (tempString != "") output.Add(tempString);
            return output.ToArray();
        }
    }
}
