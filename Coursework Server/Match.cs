using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
#pragma warning disable
    class Match
    {
        public string[] players;
        int gameTime;
        /// <summary>
        /// Creates a new match between two players
        /// </summary>
        /// <param name="player1">One of the players</param>
        /// <param name="player2">The other player</param>
        public Match(Client player1, Client player2)
        {
            players = new string[] { player1.userName, player2.userName };
            gameTime = 0;
            
        }
        public override bool Equals(object obj)
        {
            return players.Equals(((Match)obj).players);
        }
    }
}
