using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CourseworkClient
{
    public struct Effect : IComparable
    {
        public string name;
        public string description;
        public Color colour;
        public static List<Effect> allEffects = new List<Effect>();
        public Effect(string n, string d, string colourName)
        {
            colour = GetColourFromName(colourName);
            name = n;
            description = d;
        }
        public static Effect GetEffect(string s) //Replace this with a binary search if i get time
        {
            foreach (Effect e in allEffects)
            {
                if (e.name == s) return e;
            }
            throw new ArgumentException();
        }
        static Color GetColourFromName(string name)
        {
            switch (name)
            {
                case "Black":
                    return Color.Black;
                case "Red":
                    return Color.Red;
                case "DarkRed":
                    return Color.DarkRed;
                case "DarkGreen":
                    return Color.DarkGreen;
                case "LightGray":
                    return Color.LightGray;
                case "Blue":
                    return Color.Blue;
                case "LightGreen":
                    return Color.LightGreen;
                case "Purple":
                    return Color.Purple;
                case "DarkGray":
                    return Color.DarkGray;
                case "Gray":
                    return Color.Gray;
                default:
                    return Color.White;
            }
        }
        public static bool operator ==(Effect a, Effect b)
        {
            return a.name == b.name;
        }
        public static bool operator !=(Effect a, Effect b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            else if (this == null || obj == null) return false;
            else return (Effect)obj == this;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int CompareTo(object obj) //For if i use a binary search
        {
            return ((Effect)obj).name.CompareTo(name);
        }
    }
}
