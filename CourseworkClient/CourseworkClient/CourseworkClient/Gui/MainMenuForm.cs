using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    class MainMenuForm : Form
    {
        /// <summary>
        /// Creates a new MainMenuForm
        /// </summary>
        public MainMenuForm()
        {
            background = Primary.game.mainMenuBackground;
            formItems.Add(new FormChangeButton(new Rectangle(0, 300, 200, 50), "Options", FormChangeButtonTypes.MainMenuToOptions));
            formItems.Add(new FormChangeButton(new Rectangle(0, 100, 200, 50), "Store", FormChangeButtonTypes.MainMenuToStore));
            formItems.Add(new FormChangeButton(new Rectangle(0, 0, 200, 50), "Play", FormChangeButtonTypes.MainMenuToQueue));
            formItems.Add(new FormChangeButton(new Rectangle(0, 200, 200, 50), "Deck Manager", FormChangeButtonTypes.MainMenuToDeckManager));
            formItems.Add(new LeaderboardButton(new Rectangle(0, 400, 200, 50)));
            formItems.Add(new ExitButton(new Rectangle(0, 500, 200, 50)));
        }
        
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.DrawString(Primary.game.mainFont, string.Format("Elo - {0}\nCoins - {1}", Primary.game.elo, Primary.game.coins), new Vector2(300, 300), Color.White);
        }

        public override void Update()
        {
            base.Update();
        }

    }
}
