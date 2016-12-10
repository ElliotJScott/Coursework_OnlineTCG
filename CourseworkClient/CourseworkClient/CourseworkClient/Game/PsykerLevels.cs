using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Game
{
    public class PsykerLevel
    {
        //αβγδεζηθιπψω
        char character;
        int level;
        public static PsykerLevel[] levels =
            {
            new PsykerLevel('α', 5),
            new PsykerLevel('β', 5),
            new PsykerLevel('γ', 4),
            new PsykerLevel('δ', 4),
            new PsykerLevel('ε', 3),
            new PsykerLevel('ζ', 3),
            new PsykerLevel('η', 2),
            new PsykerLevel('θ', 2),
            new PsykerLevel('ι', 2),
            new PsykerLevel('κ', 1),
            new PsykerLevel('π', 0),
            new PsykerLevel('ψ', -1),
            new PsykerLevel('ω', -2),
        };
        public PsykerLevel(char c, int l)
        {
            character = c;
            level = l;
        }
        public int GetBonus()
        {
            if (level >= 1)
                return (int)Math.Round(Math.Pow(Math.PI / 2d, level));
            else return 0;
        }
        public int GetHPBonus()
        {
            if (level >= 1)
                return (int)Math.Round(Math.Pow(Math.PI/2.5d, level));        
            else return 0;
        }
        
    }
}
