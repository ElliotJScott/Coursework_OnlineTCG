using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    class CreateAccountForm : Form
    {
        public string errorMessageText = "";
        public CreateAccountForm()
        {
            background = Primary.game.loginScreenBackground;
            //Don't put any adds here
            formItems.Add(new TextField(new Rectangle((Primary.game.GraphicsDevice.Viewport.Width - Primary.game.textFieldTexture.Width) / 2, -30 + ((Primary.game.GraphicsDevice.Viewport.Height - Primary.game.textFieldTexture.Height) / 2), Primary.game.textFieldTexture.Width, Primary.game.textFieldTexture.Height), 15, "Username"));
            formItems.Add(new TextField(new Rectangle((Primary.game.GraphicsDevice.Viewport.Width - Primary.game.textFieldTexture.Width) / 2, 30 + ((Primary.game.GraphicsDevice.Viewport.Height - Primary.game.textFieldTexture.Height) / 2), Primary.game.textFieldTexture.Width, 30), 15, "Password", true));
            formItems.Add(new TextField(new Rectangle((Primary.game.GraphicsDevice.Viewport.Width - Primary.game.textFieldTexture.Width) / 2, 90 + ((Primary.game.GraphicsDevice.Viewport.Height - Primary.game.textFieldTexture.Height) / 2), Primary.game.textFieldTexture.Width, 30), 15, "Confirm Password", true));
            formItems.Add(new CreateAccountButton(new Rectangle(0, 0, 150, 30), "Create Account"));
            formItems.Add(new FormChangeButton(new Rectangle(0, 40, 150, 30), "Back", FormChangeButtonTypes.CreateAccountToLogIn));
        }
        public bool PasswordsMatch()
        {
            return ((TextField)formItems[1]).text == ((TextField)formItems[2]).text;
        }
        public override void Update()
        {
            base.Update();
            if (Primary.game.keypresshandler.HasKeyBeenPressed(Keys.Tab))
            {
                for (int i = 0; i < 3; i++)
                {
                    if (((TextField)formItems[i]).selected)
                    {
                        ((TextField)formItems[i]).selected = false;
                        ((TextField)formItems[(i+1)%3]).selected = true;
                        break;
                    }
                }
            }
            if (Primary.game.keypresshandler.HasKeyBeenPressed(Keys.Enter)) ((Button)formItems[3]).OnPress();
            
            //if (!Primary.game.connected)
            //    errorMessageText = "Error connecting to server";
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            Texture2D titleTex = Primary.game.title;
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            sb.Draw(Primary.game.title, new Rectangle((v.Width - titleTex.Width) / 2, (-115 - titleTex.Height) + (v.Height / 2), Primary.game.title.Width, Primary.game.title.Height), Color.White);
            sb.DrawString(Primary.game.mainFont, errorMessageText, new Vector2((Primary.game.GraphicsDevice.Viewport.Width / 2) - 150, (Primary.game.GraphicsDevice.Viewport.Height /2) + 110), Color.Blue);
        }
        public override int GetFormID() => 1;
    }
}
