using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    class Card
    {
        string name;
        int? attack, defence;
        int type;
        List<Effect> effects;
        public static List<Card> allCards = new List<Card>();
        public bool hasEffect(string s)
        {
            foreach (Effect e in effects)
            {
                if (e.name == s) return true;
            }
            return false;
        }
    }
}
