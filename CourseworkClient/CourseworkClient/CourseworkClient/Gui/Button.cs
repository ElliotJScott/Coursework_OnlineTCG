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
        public Button(Rectangle rect, string text)
        {
            texture = Primary.game.buttonTexture;
            buttonText = text;
            boundingBox = rect;
        }
        public Button() { }
        public string buttonText;
        public abstract void OnPress();
        public bool previouslyClicked = false;
        public override void Update()
        {
            MouseState currentState = Mouse.GetState();
            if (Clicked())
            {
                OnPress();
                previouslyClicked = true;
            }
            else if (currentState.LeftButton == ButtonState.Released && oldState.LeftButton == ButtonState.Pressed && previouslyClicked) previouslyClicked = false;
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(texture == null ? Primary.game.buttonTexture : texture, boundingBox, previouslyClicked ? Color.Orange : Color.White);
            sb.DrawString(Primary.game.mainFont, buttonText, new Vector2(boundingBox.X + 5, boundingBox.Y + 5), previouslyClicked ? Color.White : Color.Black);
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

    class CreateAccountButton : Button
    {
        public CreateAccountButton(Rectangle rect, string text) : base(rect, text) { }

        public override void OnPress()
        {
            for (int i = 0; i < 3; i++)
            {
                if (((TextField)((CreateAccountForm)Primary.game.currentForm).formItems[i]).text == "")
                {
                    ((CreateAccountForm)Primary.game.currentForm).errorMessageText = "Field cannot be empty";
                    return;
                }
            }
            if (!((CreateAccountForm)Primary.game.currentForm).PasswordsMatch())
            {
                ((CreateAccountForm)Primary.game.currentForm).errorMessageText = "Passwords do not match";
                return;
            }
            else
            {
                ((CreateAccountForm)Primary.game.currentForm).errorMessageText = "";
                string stringToWrite = ((TextField)((CreateAccountForm)Primary.game.currentForm).formItems[0]).text + "|" + Primary.game.ComputeHash(((TextField)((CreateAccountForm)Primary.game.currentForm).formItems[1]).text);
                Primary.game.WriteDataToStream(Protocol.CreateAccount, stringToWrite);
                Primary.game.currentForm.Lock("Checking username availability");
            }

            //throw new NotImplementedException();
        }
    }

    class AddToQueueButton : FormChangeButton
    {
        int queueID;
        public AddToQueueButton(Rectangle rect, string text, int id, FormChangeButtonTypes f = FormChangeButtonTypes.QueueSelectToMainMenu) : base(rect, text, f)
        {
            queueID = id;
        }

        public override void OnPress()
        {
            Primary.game.WriteDataToStream(Protocol.AddToQueue, queueID.ToString());
            Primary.game.currentForm = FormBuilder.BuildNewForm(FormChangeButtonTypes.QueueSelectToMainMenu);
        }
    }

    class LogInButton : Button
    {
        public LogInButton(Rectangle rect, string text) : base(rect, text) { }
        public override void OnPress()
        {
            for (int i = 0; i < 2; i++)
            {
                if (((TextField)((LoginScreenForm)Primary.game.currentForm).formItems[i]).text == "")
                {
                    ((LoginScreenForm)Primary.game.currentForm).errorMessageText = "Field cannot be empty";
                    return;
                }
            }
             ((LoginScreenForm)Primary.game.currentForm).errorMessageText = "";
            string stringToWrite = ((TextField)((LoginScreenForm)Primary.game.currentForm).formItems[0]).text + "|" + Primary.game.ComputeHash(((TextField)((LoginScreenForm)Primary.game.currentForm).formItems[1]).text);
            Primary.game.WriteDataToStream(Protocol.LogIn, stringToWrite);
            Primary.game.currentForm.Lock("Checking credentials");
        }
    }

    class ExitButton : Button
    {
        public ExitButton(Rectangle rect)
        {
            boundingBox = rect;
            buttonText = "Exit";
        }
        public override void OnPress()
        {
            Primary.game.Exit();
        }
    }

    class SendButton : Button
    {
        public SendButton(Rectangle rect)
        {
            boundingBox = rect;
            buttonText = "Send";
        }
        public override void OnPress()
        {
            System.Windows.Forms.MessageBox.Show("Not implemented yet!");
        }
    }

    class AddFriendButton : Button
    {
        public AddFriendButton(Rectangle rect)
        {
            boundingBox = rect;
            buttonText = "Add Friend";
        }
        public override void OnPress()
        {
            System.Windows.Forms.MessageBox.Show("Not implemented yet!");
        }
    }

    abstract class BigCardActionButton : Button
    {
        public bool canBePressed;

        public BigCardActionButton(Rectangle r, Texture2D t)
        {
            buttonText = "";
            texture = t;
            boundingBox = r;
        }

        public override void Draw(SpriteBatch sb)
        {
            Color c;
            if (canBePressed)
                c = previouslyClicked ? Color.Orange : Color.White;
            else c = Color.Black;
            sb.Draw(texture == null ? Primary.game.buttonTexture : texture, boundingBox, c);
        }
    }

    class BCPlayButton : BigCardActionButton
    {
        public BCPlayButton(Rectangle r, Texture2D t) : base(r, t) { }
        public override void OnPress()
        {
            if (canBePressed)
            {
               
                BigCard bigcard = ((InGameForm)Primary.game.currentForm).bigCard;
                Card card = bigcard.card;
                ((InGameForm)Primary.game.currentForm).PlaySelectedCard();
                ((InGameForm)Primary.game.currentForm).bigCard = null;
            }
        }
    }

    class BCDiscardButton : BigCardActionButton
    {
        public BCDiscardButton(Rectangle r, Texture2D t) : base(r, t) { }
        public override void OnPress()
        {
            if (canBePressed)
            {
                BigCard bigcard = ((InGameForm)Primary.game.currentForm).bigCard;
                Card card = bigcard.card;
                ((InGameForm)Primary.game.currentForm).DiscardSelectedCard();
                ((InGameForm)Primary.game.currentForm).bigCard = null;
            }
        }
    }

    class BCAttackButton : BigCardActionButton
    {
        public BCAttackButton(Rectangle r, Texture2D t) : base(r, t) { }
        public override void OnPress()
        {
            if (canBePressed)
            {
                BigCard bigcard = ((InGameForm)Primary.game.currentForm).bigCard;
                Card card = bigcard.card;
            }
        }
    }
    class IGSelectButton : BigCardActionButton
    {
        Selection[] selection;
        public IGSelectButton(Rectangle r, Selection[] s) : base(r, Primary.game.buttonTexture)
        {
            buttonText = "Counter";
            selection = s;
        }
        public IGSelectButton(Rectangle r) : base(r, Primary.game.buttonTexture)
        {
            buttonText = "Defend";
            selection = new Selection[] { new Selection(1, true, SelectionCondition.alliedUntappedUnit)};

        }
        public override void OnPress()
        {
            Primary.game.currentForm = new SelectionForm((InGameForm)Primary.game.currentForm, selection);
            
        }
    }
    class IGCancelButton : Button
    {
        public IGCancelButton(Rectangle r) : base(r, "Make no selection") { }

        public override void OnPress()
        {
            ((InGameForm)Primary.game.currentForm).ResolveChain();
            Primary.game.SendData(new byte[] { (byte)Protocol.NoCounter});
        }
    }
}
