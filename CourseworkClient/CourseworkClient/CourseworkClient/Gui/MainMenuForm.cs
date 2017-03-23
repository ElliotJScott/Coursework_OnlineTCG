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
            formItems.Add(new FormChangeButton(new Rectangle(2, 2, 100, 50), "Options", FormChangeButtonTypes.MainMenuToOptions));
            formItems.Add(new FormChangeButton(new Rectangle(2, 200, 100, 50), "Store", FormChangeButtonTypes.MainMenuToStore));
            formItems.Add(new FormChangeButton(new Rectangle(2, 100, 100, 50), "Play", FormChangeButtonTypes.MainMenuToQueueSelect));
            formItems.Add(new FormChangeButton(new Rectangle(2, 300, 100, 50), "Deck Manager", FormChangeButtonTypes.MainMenuToDeckManager));
        }
        
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

        public override void Update()
        {
            base.Update();
        }

    }
}
