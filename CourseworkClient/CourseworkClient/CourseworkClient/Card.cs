using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    public enum Rarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Legendary = 3,
        Unobtainable = 4,
    }
    public enum CardType
    {
        Unit = 0,
        Tech = 1,
        Upgrade = 2,
    }
    public struct Card
    {
        public int cost;
        public string name;
        public int? attack, health;
        public CardType type;
        public List<Effect> effects;
        public static List<Card> allCards = new List<Card>();
        public Rarity rarity;
        public List<Card> equippedUpgrades;

        public Card(string n, CardType t, int c, Rarity r, int? atk = null, int? hp = null, params Effect[] e)
        {
            cost = c;
            rarity = r;
            name = n;
            type = t;
            attack = atk;
            health = hp;
            effects = e.ToList();
            Primary.game.AddNewCardArt(n);
            equippedUpgrades = new List<Card>();
        }
        public static void AddEffectToBaseCard(string cardname, string effectname)
        {
            allCards[getCardIndex(cardname)].effects.Add(Effect.GetEffect(effectname));
        }
        public List<string> getEffectNames()
        {
            List<string> output = new List<string>();
            foreach (Effect e in effects) output.Add(e.name);
            return output;
        }
        public List<Color> getColourList()
        {
            List<Color> output = new List<Color>();
            foreach (Effect e in effects) output.Add(e.colour);
            return output;
        }
        public static int getCardIndex(string name)
        {
            for (int i = 0; i < allCards.Count; i++)
            {
                if (allCards[i].name == name) return i;
            }
            throw new ArgumentException();
        }
        public static Card getCard(string name)
        {
            foreach (Card c in allCards)
            {
                if (c.name == name) return c;
            }
            throw new ArgumentException();
        }
        public bool hasEffect(string effectName) => effects.Contains(Effect.GetEffect(effectName));
    }
}
