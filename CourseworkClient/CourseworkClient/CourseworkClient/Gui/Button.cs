using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    abstract class Button : GuiItem
    {
        public string buttonText;
        public abstract void OnPress();
        public override void Update()
        {
            if (Clicked(new Vector2(Mouse.GetState().X, Mouse.GetState().Y))) OnPress();
        }
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, boundingBox, Color.White);
            sb.DrawString(Primary.game.mainFont, buttonText, new Vector2(boundingBox.X + 5, boundingBox.Y + 5), Color.Black);
        }
    }
    class FormChangeButton : Button
    {
        Form newForm;
        public FormChangeButton(Texture2D tex, string text, Form form)
        {
            texture = tex;
            buttonText = text;
            newForm = form;
        }

        public override void OnPress()
        {
            Primary.game.currentForm = newForm;
        }

    }

    class TransmissionButton : Button
    {
        public override void OnPress()
        {

        }
    }
    class ExitButton : Button
    {
        public override void OnPress()
        {
            Primary.game.Exit();
        }
    }
}
