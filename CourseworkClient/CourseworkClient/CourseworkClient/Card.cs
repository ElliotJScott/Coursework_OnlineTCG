using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    /// <summary>
    /// The rarity of a card
    /// </summary>
    public enum Rarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Legendary = 3,
        Unobtainable = 4,
    }
    /// <summary>
    /// What sort of card a card is
    /// </summary>
    public enum CardType
    {
        Unit = 0,
        Tech = 1,
        Upgrade = 2,
    }
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    public struct Card
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        public int cost;
        public string name;
        public int? attack, health;
        public CardType type;
        public List<Effect> effects;
        public static List<Card> allCards = new List<Card>();
        public Rarity rarity;
        /// <summary>
        /// Creates a new Card
        /// </summary>
        /// <param name="n">The name of the card</param>
        /// <param name="t">The type of card</param>
        /// <param name="c">The cost of the card to play</param>
        /// <param name="r">The rarity of the card</param>
        /// <param name="atk">The attack of the card. Null for non-units</param>
        /// <param name="hp">The HP of the card. Null for non-units</param>
        /// <param name="e">An array of the effects that the card has</param>
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
            Primary.Log("Creating card " + this);
        }
        /// <summary>
        /// Adds an effect to a card
        /// </summary>
        /// <param name="cardname">The name of the card to get the effect</param>
        /// <param name="effectname">The name of the effect</param>
        public static void AddEffectToBaseCard(string cardname, string effectname)
        {
            allCards[getCardIndex(cardname)].effects.Add(Effect.GetEffect(effectname));
        }
        /// <summary>
        /// Gets the names of all the effects that this card has
        /// </summary>
        /// <returns>A list of the effect names</returns>
        public List<string> getEffectNames()
        {
            Primary.Log("Getting effects of " + name);
            List<string> output = new List<string>();
            foreach (Effect e in effects) output.Add(e.name);
            return output;
        }
        /// <summary>
        /// Gets all the colours of the effects that this card has (For drawing)
        /// </summary>
        /// <returns>A list of the effect colours</returns>
        public List<Color> getColourList()
        {
            Primary.Log("Getting colours for " + name);
            List<Color> output = new List<Color>();
            foreach (Effect e in effects) output.Add(e.colour);
            return output;
        }
        /// <summary>
        /// Gets the index of a card in the list of all cards
        /// </summary>
        /// <param name="name">The name of the card</param>
        /// <returns>The index of the card</returns>
        public static int getCardIndex(string name)
        {
            Primary.Log("Indexof " + name);
            for (int i = 0; i < allCards.Count; i++)
            {
                if (allCards[i].name == name) return i;
            }
            throw new ArgumentException();
        }
        /// <summary>
        /// Gets the card with a given name
        /// </summary>
        /// <param name="name">The name of the card</param>
        /// <returns>The card with the given name</returns>
        public static Card getCard(string name)
        {
            Primary.Log("Getting card " + name);
            foreach (Card c in allCards)
            {
                if (c.name == name) return c;
            }
            throw new ArgumentException();
        }
        /// <summary>
        /// Gets whether this card has an effect
        /// </summary>
        /// <param name="effectName">The name of the effect to check</param>
        /// <returns>Whether the card has the given effect</returns>
        public bool hasEffect(string effectName) => effects.Contains(Effect.GetEffect(effectName));
        public override string ToString()
        {
            return string.Format("Card[Name : {0} | Cost : {1} | Rarity : {2} | Type : {3}]", name, cost, rarity, type);
        }
        public static bool operator ==(Card a, Card b) => a.name == b.name;
        public static bool operator !=(Card a, Card b) => !(a==b);

    }
}
