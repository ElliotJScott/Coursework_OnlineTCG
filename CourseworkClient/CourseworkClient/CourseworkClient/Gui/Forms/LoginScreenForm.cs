using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace CourseworkClient.Gui
{
    class LoginScreenForm : Form
    {
        public string errorMessageText = "";
        public LoginScreenForm()
        {
            background = Primary.game.loginScreenBackground;
            //Don't put any adds here
            formItems.Add(new TextField(new Rectangle((Primary.game.GraphicsDevice.Viewport.Width - Primary.game.textFieldTexture.Width) / 2, -30 + ((Primary.game.GraphicsDevice.Viewport.Height - Primary.game.textFieldTexture.Height) / 2), Primary.game.textFieldTexture.Width, Primary.game.textFieldTexture.Height), 15, "Username"));
            formItems.Add(new TextField(new Rectangle((Primary.game.GraphicsDevice.Viewport.Width - Primary.game.textFieldTexture.Width) / 2, 30 + ((Primary.game.GraphicsDevice.Viewport.Height - Primary.game.textFieldTexture.Height) / 2), Primary.game.textFieldTexture.Width, 30), 15, "Password", true));
            formItems.Add(new LogInButton(new Rectangle(0, 0, 150, 30),"Log In")); //Change this null when i have the thing
            formItems.Add(new FormChangeButton(new Rectangle(0, 40, 150, 30), "Create Account", FormChangeButtonTypes.LogInToCreateAccount));
        }
        public override void Update()
        {
            base.Update();
            if (!Primary.game.connected && errorMessageText == "")
                errorMessageText = "Error establishing connection to server";
            if (((TextField)formItems[0]).selected || ((TextField)formItems[1]).selected)
            {
                if (Primary.game.keypresshandler.HasKeyBeenPressed(Keys.Tab))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        ((TextField)formItems[i]).selected = !((TextField)formItems[i]).selected;
                    }
                }
            }
            if (Primary.game.keypresshandler.HasKeyBeenPressed(Keys.Enter))
            {
                ((Button)formItems[2]).OnPress();
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            Texture2D titleTex = Primary.game.title;
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            sb.Draw(Primary.game.title, new Rectangle((v.Width - titleTex.Width) / 2,(-115-titleTex.Height) + (v.Height/2), Primary.game.title.Width, Primary.game.title.Height), Color.White);
            sb.DrawString(Primary.game.mainFont, errorMessageText, new Vector2((Primary.game.GraphicsDevice.Viewport.Width / 2) - 150, (Primary.game.GraphicsDevice.Viewport.Height / 2) + 110), Color.Blue);

        }

        public override int GetFormID() => 0;

    }
}
