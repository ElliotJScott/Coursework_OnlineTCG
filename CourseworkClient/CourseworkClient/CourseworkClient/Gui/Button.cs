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
        bool previouslyClicked = false;
        public override void Update()
        {
            MouseState currentState = Mouse.GetState();
            if (Clicked(new Vector2(currentState.X, currentState.Y)))
            {
                OnPress();
                previouslyClicked = true;
            }
            else if (currentState.LeftButton == ButtonState.Released && oldState.LeftButton == ButtonState.Pressed && previouslyClicked) previouslyClicked = false;
        }
        
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture, boundingBox, previouslyClicked? Color.Orange : Color.White);
            sb.DrawString(Primary.game.mainFont, buttonText, new Vector2(boundingBox.X + 5, boundingBox.Y + 5), previouslyClicked? Color.White : Color.Black);
        }
    }
    class FormChangeButton : Button
    {
        FormChangeButtonTypes type;
        public FormChangeButton(Rectangle rect, string text, FormChangeButtonTypes f)
        {
            type = f;
            texture = Primary.game.buttonTexture;
            buttonText = text;            
            boundingBox = rect;
        }
        

        public override void OnPress()
        {
            Primary.game.currentForm = FormBuilder.BuildNewForm(type);
        }

    }

    class TransmissionButton : Button
    {
        string transmissionString;
        public TransmissionButton(Rectangle rect, string text)
        {
            texture = Primary.game.buttonTexture;
            buttonText = text;
            boundingBox = rect;
        }
        public void SetTransmissionString(string s)
        {
            transmissionString = s;
        }
        public override void OnPress()
        {
            //Transmit
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
