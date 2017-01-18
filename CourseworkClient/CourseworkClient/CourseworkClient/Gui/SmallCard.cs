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
        public int id = -1;
        
        public SmallCard(Card c, Vector2 l)
        {
            tapped = false;
            wasDrawnBig = false;
            card = c;
            boundingBox = new Rectangle((int)l.X, (int)l.Y, Primary.game.cardOutlineSmall.Width, Primary.game.cardOutlineSmall.Height);
            drawnBig = false;
        }
        public SmallCard(Card c, int ID, bool tap)
        {
            wasDrawnBig = false;
            card = c;
            drawnBig = false;
            id = ID;
            tapped = tap;
        }
        public SmallCard CloneWithoutReferenceForSelection()
        {
            return new SmallCard(card, id, tapped);
        }
        public void UpdateLocation(Vector2 v)
        {
            boundingBox.X = (int)v.X;
            boundingBox.Y = (int)v.Y;
        }
        public void Draw(SpriteBatch sb, Orientation o)
        {
            if (!drawnBig)
            {
                CardBuilder.DrawCard(card, new Vector2(boundingBox.X, boundingBox.Y), false, sb, o);
            }
            else
            {
                //Console.Write("e");
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            Draw(sb, Orientation.Up);
        }
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
                        ((SelectionForm)Primary.game.currentForm).bigCard = new BigCard(card);
                    }
                }
                wasDrawnBig = false;
            }
            //Update this
        }
        public void StopDrawBig(SmallCard c)
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
