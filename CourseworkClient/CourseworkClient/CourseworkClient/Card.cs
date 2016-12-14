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
    public struct Card
    {
        public string name;
        public int? attack, health;
        public int type;
        public List<Effect> effects;
        public static List<Card> allCards = new List<Card>();
        public const int unitType = 0;
        public const int linusTechType = 1;
        public const int upgradeType = 2;
        public Rarity rarity;
        public List<Card> equippedUpgrades;

        public Card(string n, int t, Rarity r, int? atk = null, int? hp = null, params Effect[] e)
        {
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
        public bool hasEffect(string s)
        {
            foreach (Effect e in effects)
            {
                if (e.name == s) return true;
            }
            return false;
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
    }
}
