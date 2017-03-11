using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    /// <summary>
    /// Used just to draw cards
    /// </summary>
    class CardBuilder
    {
        const int cardArtDispX = 12;
        const int cardArtDispY = 52;
        const int maxNamePixelWidth = 140;
        const int maxCharsPerNameLine = 16;
        /// <summary>
        /// Draw a card
        /// </summary>
        /// <param name="c">The card to draw</param>
        /// <param name="pos">The position on the screen to draw the card</param>
        /// <param name="big">Whether or not to draw the card as a BigCard</param>
        /// <param name="sb">The SpriteBatch used to draw the card</param>
        /// <param name="orientation">What orientation to draw the card. May not include this in the final version because it is difficult to accurately draw the different images in the right places when rotated</param>
        public static void DrawCard(Card c, Vector2 pos, bool big, SpriteBatch sb, bool playerPlayed, bool tapped) //Note this doesn't include the text which only applies to the big cards
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
            float rotation = playerPlayed ? 0f : ((float)Math.PI);
            pos += !playerPlayed && !big? new Vector2(cardOutline.Width, cardOutline.Height) : new Vector2(0);
            Vector2 cardCentre = new Vector2(0, 0);
            sb.Draw(innerTexture, new Rectangle((int)pos.X, (int)pos.Y, innerTexture.Width, innerTexture.Height), null, Color.White, rotation, cardCentre, SpriteEffects.None, 1);
            int disp = big ? 13 : 6;
            Texture2D cardArt = Primary.game.GetCardArt(c.name);
            int artX = big ? (int)pos.X + cardArtDispX : (int)pos.X + (cardArtDispX / 2);
            int artY = big ? (int)pos.Y + cardArtDispY : (int)pos.Y + (cardArtDispY / 2);
            if (!playerPlayed && !big)
            {
                artX += cardArt.Width;
                artY += cardArt.Height;
            }
            sb.Draw(cardArt, new Rectangle(artX, artY, big ? cardArt.Width : cardArt.Width / 2, big ? cardArt.Height : cardArt.Height / 2), null, Color.White, rotation, cardCentre, SpriteEffects.None, 1);
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
                if (Primary.game.currentForm.GetType() == typeof(InGameForm))
                {
                    InGameForm currentForm = (InGameForm)Primary.game.currentForm;
                    List<string> upgradeNames = new List<string>();
                    SmallCard sc = currentForm.GetDrawnSmallCard();
                    foreach (Upgrade u in currentForm.upgradesInPlay)
                    {
                        if (u.unitID == sc.id)
                        {
                            upgradeNames.Add(currentForm.GetUpgradeFromID(u.upgradeID, null).card.name);
                        }
                    }
                    sb.DrawString(Primary.game.mainFont, "Equipped Upgrades:", new Vector2((int)pos.X, cardOutline.Height + (int)pos.Y), Color.Red);
                    for (int i = 0; i < upgradeNames.Count; i++)
                    {
                        sb.DrawString(Primary.game.mainFont, upgradeNames[i], new Vector2((int)pos.X, ((i + 1) * 100) + cardOutline.Height + (int)pos.Y), Color.Red);
                    }
                }
            }
            sb.Draw(cardOutline, new Rectangle((int)pos.X, (int)pos.Y, cardOutline.Width, cardOutline.Height), null, Color.White, rotation, cardCentre, SpriteEffects.None, 1);
            if (tapped && !big)
            {
                sb.Draw(Primary.game.cardTappedIndicator, new Rectangle((int)pos.X, (int)pos.Y, cardOutline.Width, cardOutline.Height), Color.White);
            }
        }
        /// <summary>
        /// Works out what scaling to draw a string at in the given font. Note the recursion used here.
        /// </summary>
        /// <param name="str">The string to draw</param>
        /// <param name="font">The font the string will be drawn in</param>
        /// <param name="width">The maximum width that the string can be drawn at in pixels</param>
        /// <param name="height">The maximum height that the string can be drawn at in pixels</param>
        /// <param name="max">The scaling currently being tested. Initialised at 30</param>
        /// <returns>The scaling the string will be drawn at</returns>
        static float CalculateScale(string str, SpriteFont font, int width, int height = int.MaxValue, float max = 30f)
        {
            Vector2 v = font.MeasureString(str) * max;
            if (v.X > width || v.Y > height) return CalculateScale(str, font, width, height, max - 0.05f);
            else return max;
        }
        /// <summary>
        /// Splits a number of words neatly into two lines.
        /// </summary>
        /// <param name="words">The words to be splitted</param>
        /// <returns>The two lines produced</returns>
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
