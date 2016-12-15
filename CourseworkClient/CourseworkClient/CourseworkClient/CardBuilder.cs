using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    class CardBuilder
    {
        const int cardArtDispX = 10;
        const int cardArtDispY = 20;
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
                    case 0:
                        innerTexture = Primary.game.unitBigInner;
                        break;
                    case 1:
                        innerTexture = Primary.game.techBigInner;
                        break;
                    case 2:
                        innerTexture = Primary.game.upgradeBigInner;
                        break;
                }
            }
            else
            {
                cardOutline = Primary.game.cardOutlineSmall;
                switch (c.type)
                {
                    case 0:
                        innerTexture = Primary.game.unitSmallInner;
                        break;
                    case 1:
                        innerTexture = Primary.game.techSmallInner;
                        break;
                    case 2:
                        innerTexture = Primary.game.upgradeSmallInner;
                        break;

                }
            }
            #endregion
            sb.Draw(cardOutline, new Rectangle((int)pos.X, (int)pos.Y, cardOutline.Width, cardOutline.Height), Color.White);
            sb.Draw(innerTexture, new Rectangle((int)pos.X, (int)pos.Y, innerTexture.Width, innerTexture.Height), Color.White);
            int disp = big ? 10 : 5;
            Texture2D cardArt = Primary.game.GetCardArt(c.name);
            sb.Draw(cardArt, new Rectangle(big ? (int)pos.X + cardArtDispX : (int)pos.X + (cardArtDispX / 2), big ? (int)pos.Y + cardArtDispY : (int)pos.Y + (cardArtDispY / 2), big ? cardArt.Width : cardArt.Width / 2, big ? cardArt.Height : cardArt.Height / 2), Color.White);
            sb.DrawString(Primary.game.mainFont, c.name, pos + new Vector2(disp, disp), Color.Black);
        }
    }
}
