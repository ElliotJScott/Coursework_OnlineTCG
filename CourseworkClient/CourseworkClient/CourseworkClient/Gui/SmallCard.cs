using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CourseworkClient.Gui
{
    class SmallCard : GuiItem
    {
        public Card card;
        public bool tapped;
        public bool drawnBig;
        public bool wasDrawnBig;
        public int id;
        
        /// <summary>
        /// Creates a new SmallCard
        /// </summary>
        /// <param name="c">The card to create the SmallCard from</param>
        /// <param name="l">The position of the SmallCard</param>
        public SmallCard(Card c, int ID, Vector2 l)
        {
            id = ID;
            tapped = false;
            wasDrawnBig = false;
            card = c;
            boundingBox = new Rectangle((int)l.X, (int)l.Y, Primary.game.cardOutlineSmall.Width, Primary.game.cardOutlineSmall.Height);
            drawnBig = false;
        }
        /// <summary>
        /// Creates a new SmallCard
        /// </summary>
        /// <param name="c">The card to create the SmallCard from</param>
        /// <param name="ID">The ID of the SmallCard</param>
        /// <param name="tap">Whether or not the card is tapped</param>
        public SmallCard(Card c, int ID, bool tap)
        {
            wasDrawnBig = false;
            card = c;
            drawnBig = false;
            id = ID;
            tapped = tap;
            boundingBox = new Rectangle(0, 0, Primary.game.cardOutlineSmall.Width, Primary.game.cardOutlineSmall.Height);
        }
        /// <summary>
        /// Returns a copy of the card without the pointer
        /// </summary>
        /// <returns>The value of this card</returns>
        public SmallCard CloneWithoutReferenceForSelection()
        {
            return new SmallCard(card, id, tapped);
        }
        /// <summary>
        /// Updates the location of this card
        /// </summary>
        /// <param name="v">The new location of this card</param>
        public void UpdateLocation(Vector2 v)
        {
            boundingBox.X = (int)v.X;
            boundingBox.Y = (int)v.Y;
        }
        public void Draw(SpriteBatch sb, bool playerPlayed)
        {
            if (!drawnBig)
            {
                CardBuilder.DrawCard(card, new Vector2(boundingBox.X, boundingBox.Y), false, sb, playerPlayed, tapped);
            }
            else
            {
                //Console.Write("e");
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            Draw(sb, true);
        }
        /// <summary>
        /// Adds an effect to this card
        /// </summary>
        /// <param name="e">The effect to add</param>
        public void AddEffect(Effect e)
        {
            if (!card.effects.Contains(e))
            {
                card.effects.Add(e);
            }
        }
        public override void Update()
        {
            Type type = Primary.game.currentForm.GetType();
            if (type == typeof(InGameForm))
            {
                if (!drawnBig && !wasDrawnBig)
                {
                    MouseState m = Mouse.GetState();
                    if (Clicked())
                    {
                        foreach (SmallCard c in ((InGameForm)Primary.game.currentForm).hand) StopDrawBig(c);
                        foreach (SmallCard c in ((InGameForm)Primary.game.currentForm).units) StopDrawBig(c);
                        foreach (SmallCard c in ((InGameForm)Primary.game.currentForm).enemyUnits) StopDrawBig(c);
                        drawnBig = true;
                        ((InGameForm)Primary.game.currentForm).bigCard = new BigCard(card);
                    }
                }
                wasDrawnBig = false;
            }
            else if (type == typeof(SelectionForm))
            {
                if (!drawnBig && !wasDrawnBig)
                {
                    MouseState m = Mouse.GetState();
                    if (Clicked())
                    {
                        foreach (SmallCard c in ((SelectionForm)Primary.game.currentForm).cards) StopDrawBig(c);
                        drawnBig = true;
                        ((SelectionForm)Primary.game.currentForm).bigCard = new BigCard(card, new Vector2((Primary.game.GraphicsDevice.Viewport.Width * 3)/4, 200));
                    }
                }
                wasDrawnBig = false;
            }
            //Update this
        }
        /// <summary>
        /// Stop drawing the given card from being drawn big
        /// </summary>
        /// <param name="c">The card to stop drawing big</param>
        public static void StopDrawBig(SmallCard c)
        {
            if (c.drawnBig == true)
            {
                c.wasDrawnBig = true;
                c.drawnBig = false;
            }
        }
        public static bool operator ==(SmallCard a, SmallCard b)
        {
            if (ReferenceEquals(a, b)) return true;
            else if (a.Equals(null) || b.Equals(null)) return false;
            else return a.id == b.id && a.card.name == b.card.name;
        }
        public static bool operator !=(SmallCard a, SmallCard b) => !(a == b);
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            else if (obj.GetType() == typeof(SmallCard)) return this == (SmallCard)obj;
            else return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("SmallCard[Card : {0} | Tapped : {1} | DrawnBig : {2}", card, tapped, drawnBig);
        }
    }
}
