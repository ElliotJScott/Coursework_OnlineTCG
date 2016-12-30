using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    class CardBuilder
    {
        const int cardArtDispX = 12;
        const int cardArtDispY = 52;
        const int maxNamePixelWidth = 140;
        const int maxCharsPerNameLine = 16;
        public static void DrawCard(Card c, Vector2 pos, bool big, SpriteBatch sb) //Note this doesn't include the inner text which only applies to the big cards
        {
            #region Inner and Outer Texture Setting
            Texture2D innerTexture = null;
            Texture2D cardOutline;
            if (big)
            {
                cardOutline = Primary.game.cardOutlineBig;
                switch (c.type)
                {
                    case CardType.Unit:
                        innerTexture = Primary.game.unitBigInner;
                        break;
                    case CardType.Tech:
                        innerTexture = Primary.game.techBigInner;
                        break;
                    case CardType.Upgrade:
                        innerTexture = Primary.game.upgradeBigInner;
                        break;
                }
            }
            else
            {
                cardOutline = Primary.game.cardOutlineSmall;
                switch (c.type)
                {
                    case CardType.Unit:
                        innerTexture = Primary.game.unitSmallInner;
                        break;
                    case CardType.Tech:
                        innerTexture = Primary.game.techSmallInner;
                        break;
                    case CardType.Upgrade:
                        innerTexture = Primary.game.upgradeSmallInner;
                        break;

                }
            }
            #endregion
            sb.Draw(innerTexture, new Rectangle((int)pos.X, (int)pos.Y, innerTexture.Width, innerTexture.Height), Color.White);
            int disp = big ? 13 : 6;
            Texture2D cardArt = Primary.game.GetCardArt(c.name);
            sb.Draw(cardArt, new Rectangle(big ? (int)pos.X + cardArtDispX : (int)pos.X + (cardArtDispX / 2), big ? (int)pos.Y + cardArtDispY : (int)pos.Y + (cardArtDispY / 2), big ? cardArt.Width : cardArt.Width / 2, big ? cardArt.Height : cardArt.Height / 2), Color.White);
            if (big)
            {
                if (c.name.Length > maxCharsPerNameLine)
                {
                    string[] lines = GetLines(c.name.Split(' '));
                    float scale1 = CalculateScale(lines[0], Primary.game.cardTextFont, maxNamePixelWidth, 35, 1);
                    float scale2 = CalculateScale(lines[1], Primary.game.cardTextFont, maxNamePixelWidth, 35, 1);
                    float scale = Math.Min(scale1, scale2);
                    for (int i = 0; i < 2; i++)
                    {

                        sb.DrawString(Primary.game.cardTextFont, lines[i], pos + new Vector2(disp, -3 + disp + (15 * i)), Color.Black, 0, new Vector2(0), scale, SpriteEffects.None, 0);
                    }
                }
                else
                {
                    float scale = CalculateScale(c.name, Primary.game.cardTextFont, maxNamePixelWidth, 30, 1.2f);
                    sb.DrawString(Primary.game.cardTextFont, c.name, pos + new Vector2(disp, disp), Color.Black, 0, new Vector2(0), scale, SpriteEffects.None, 0);
                }
                sb.DrawString(Primary.game.mainFont, c.cost >= 0 ? c.cost.ToString() : "-", new Vector2(pos.X + 160, pos.Y + 15), Color.Black);
                if (c.type == 0)
                    sb.DrawString(Primary.game.mainFont, c.attack + "/" + c.health, new Vector2(pos.X + (cardOutline.Width / 2) - 20, pos.Y + cardOutline.Height - 40), Color.Black);
            }
            sb.Draw(cardOutline, new Rectangle((int)pos.X, (int)pos.Y, cardOutline.Width, cardOutline.Height), Color.White);
        }
        static float CalculateScale(string str, SpriteFont font, int width, int height = int.MaxValue, float max = 30f)
        {
            Vector2 v = font.MeasureString(str) * max;
            if (v.X > width || v.Y > height) return CalculateScale(str, font, width, height, max - 0.05f);
            else return max;
        }
        static string[] GetLines(string[] words)
        {
            
            string f = "";
            foreach (string s in words) f += s;
            int c = 0;
            string line1 = "";
            string line2 = "";
            foreach (string s in words)
            {
                c += s.Length;
                int max = Math.Max(maxCharsPerNameLine, f.Length / 2);
                if (c > max) line2 += s + " ";
                else line1 += s + " "; 
            }
            return new string[] { line1.Trim(), line2.Trim()};
        }
    }
}
