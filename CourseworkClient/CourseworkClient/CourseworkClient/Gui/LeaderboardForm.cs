using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace CourseworkClient.Gui
{
    class Player
    {
        public int elo;
        public string name;
        public Player(string s, int e)
        {
            elo = e;
            name = s;
        }
    }
    class LeaderboardForm : Form
    {
        List<Player> players = new List<Player>();
        public LeaderboardForm(string s)
        {
            formItems.Add(new FormChangeButton(new Rectangle(0, 0, 150, 50), "Main Menu", FormChangeButtonTypes.LeaderboardToMainMenu));
            string[] f = s.Split('|');
            for (int i = 0; i < f.Length; i += 2)
            {
                players.Add(new Player(f[i], Convert.ToInt32(f[i+1])));
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            
            for (int i = 0; i < players.Count; i++)
            {
                sb.DrawString(Primary.game.mainFont, string.Format("{0}: {1} - {2}", i + 1, players[i].name, players[i].elo), new Vector2(200, 20 + (i * (1 + Primary.game.mainFont.MeasureString(players[i].name).Y))), players[i].name == Primary.game.username ? Color.Green : Color.White);
            }
        }
    }
}
