using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
    class Match
    {
        public string[] players;
        int gameTime;
        public Match(Client player1, Client player2)
        {
            players = new string[] { player1.userName, player2.userName };
            gameTime = 0;
            
        }
    }
}
