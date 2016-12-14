using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            background = Primary.game.loginScreenBackground;
            formItems.Add(new FormChangeButton(new Rectangle(2, 2, 100, 50), "Options", FormChangeButtonTypes.MainMenuToOptions));
            formItems.Add(new FormChangeButton(new Rectangle(2, 200, 100, 50), "Store", FormChangeButtonTypes.MainMenuToStore));
            formItems.Add(new FormChangeButton(new Rectangle(2, 100, 100, 50), "Play", FormChangeButtonTypes.MainMenuToQueueSelect));
            formItems.Add(new FormChangeButton(new Rectangle(2, 300, 100, 50), "Deck Manager", FormChangeButtonTypes.MainMenuToDeckManager));
            formItems.Add(new SendButton(new Rectangle(2, 400, 100, 50)));
            formItems.Add(new ChatBox(new Rectangle(600, 500, 200, 200)));
            formItems.Add(new AddFriendButton(new Rectangle(2, 500, 100, 50)));
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
