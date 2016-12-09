using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
    class Match
    {
        string[] players;
        int gameTime;
        public Match(string player1, string player2)
        {
            players = new string[] { player1, player2 };
            gameTime = 0;
        }
    }
}
