using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    class InGameForm : Form
    {
        MouseState oldState;
        bool playerFocus;
        List<Card> deck = new List<Card>();
        List<Card> upgradeDeck = new List<Card>();
        List<Card> hand = new List<Card>();
        List<Card> units = new List<Card>();
        List<Card> discardPile = new List<Card>();
        int numEnemyCardsInHand;
        bool cardsInEnemyDeck = true;
        bool cardsInEnemyUpgradeDeck = true;
        List<Card> enemyDiscardPile = new List<Card>();
        List<Card> enemyUnits = new List<Card>();
        const int maxUnitsInPlay = 6;
        int yOffset;
        string enemyUsername;
        public InGameForm(Deck d, bool start, string e)
        {
            background = Primary.game.inGameBackground;
            enemyUsername = e;
            playerFocus = start;
            CalculateYOffset();
            oldState = Mouse.GetState();
            foreach (DeckItem f in d.mainDeck)
            {
                for (int i = 0; i < f.quantity; i++)
                {
                    if (f.card.type == 2) upgradeDeck.Add(f.card);
                    else deck.Add(f.card);
                }
            }
            Shuffle(deck);
            for (int i = 0; i < 5; i++) DrawACard();
        }
        public override void Draw(SpriteBatch sb)
        {
            //Draw units with upgrades
            base.Draw(sb);
        }
        private void CalculateYOffset()
        {
            if (playerFocus) yOffset = 0;
            else yOffset = -Primary.game.GraphicsDevice.Viewport.Height;
        }
        public override void Update()
        {
            CalculateYOffset();
            base.Update();
        }
        public void DrawACard()
        {
            hand.Add(deck[0]);
            formItems.Add(new SmallCard(deck[0], new Vector2(GetHandCardX(hand.Count, hand.Count - 1), yOffset + Primary.game.playSpace.Height - Primary.game.cardOutlineSmall.Height - 8)));
            deck.RemoveAt(0);
            
        }
        public static int GetHandCardX(int numCards, int ord)
        {
            int leftX = (Primary.game.GraphicsDevice.Viewport.Width - (numCards * (Primary.game.cardOutlineSmall.Width + 2))) / 2;
            return leftX + (ord * (Primary.game.cardOutlineSmall.Width + 2));
        }
        public static void Shuffle(List<Card> c)
        {
            List<ShuffleItem> items = new List<ShuffleItem>();
            foreach (Card d in c) items.Add(new ShuffleItem(d));
            c.Clear();
            items.Sort();
            foreach (ShuffleItem i in items) c.Add(i);
        }
        struct ShuffleItem : IComparable
        {
            Card item;
            int num;
            public ShuffleItem(Card i)
            {
                item = i;
                num = Primary.game.rng.Next();
            }
            public int CompareTo(object obj)
            {
                return num.CompareTo(((ShuffleItem)obj).num);
            }
            public static implicit operator Card(ShuffleItem f) => f.item;
        }
    }
}
