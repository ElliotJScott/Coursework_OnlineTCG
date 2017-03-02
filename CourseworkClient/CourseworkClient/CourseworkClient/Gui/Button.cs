using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    /// <summary>
    /// This is the base class of all buttons
    /// </summary>
    abstract class Button : GuiItem
    {

        public Button(Rectangle rect, string text)
        {
            texture = Primary.game.buttonTexture;
            buttonText = text;
            boundingBox = rect;
            Primary.Log(this);
        }
        public Button()
        {

        }
        public string buttonText; //The text on the button
        /// <summary>
        /// This is called when the button is pressed
        /// </summary>
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
        public override string ToString()
        {
            return string.Format("Button[Bounding Box : {0} | Text : {1}]", boundingBox, buttonText);
        }
    }
    /// <summary>
    /// A FormChangeButton changes the current form
    /// </summary>
    class FormChangeButton : Button
    {
        FormChangeButtonTypes type; //This is the transition to make
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
    /// <summary>
    /// Sends the username and password to try to create an account, if the inputs are all in order.
    /// </summary>
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
    /// <summary>
    /// Adds the player to the queue when pressed
    /// </summary>
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
    /// <summary>
    /// Sends the credentials to check their validity when pressed
    /// </summary>
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
    /// <summary>
    /// Closes the program when pressed
    /// </summary>
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
    /// <summary>
    /// Not implemented yet.
    /// </summary>
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
    /// <summary>
    /// Not implemented yet.
    /// </summary>
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
    /// <summary>
    /// For buttons that have textures other than the default or can't always be pressed
    /// </summary>
    abstract class TexturedButton : Button
    {
        public bool canBePressed;

        public TexturedButton(Rectangle r, Texture2D t)
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
    /// <summary>
    /// Plays the currently selected BigCard
    /// </summary>
    class BCPlayButton : TexturedButton
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
    /// <summary>
    /// Discards the currently selected BigCard
    /// </summary>
    class BCDiscardButton : TexturedButton
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
    /// <summary>
    /// Attack with the currently selected BigCard
    /// </summary>
    class BCAttackButton : TexturedButton
    {
        public BCAttackButton(Rectangle r, Texture2D t) : base(r, t) { }
        public override void OnPress()
        {
            if (canBePressed)
            {
                InGameForm currentForm = ((InGameForm)Primary.game.currentForm);
                SmallCard card = currentForm.GetDrawnSmallCard();
                currentForm.chain.AddLast(new ChainItem(card, true, false));
                Primary.game.WriteDataToStream(Protocol.AttackWithUnit, card.id.ToString());
            }
        }
    }
    /// <summary>
    /// Changes the form to SelectionForm.
    /// </summary>
    class IGSelectButton : TexturedButton
    {
        SelectionItem? selection;
        public IGSelectButton(Rectangle r, string text, SelectionItem? s) : base(r, Primary.game.buttonTexture)
        {
            buttonText = text;
            selection = s;
            if (s.Equals(null))
                canBePressed = true;
            else canBePressed = false;
        }
        /// <summary>
        /// Special case button for defending against an attack with a unit
        /// </summary>
        /// <param name="r">The bounding box of the button</param>
        public IGSelectButton(Rectangle r) : base(r, Primary.game.buttonTexture)
        {
            buttonText = "Defend";
            selection = new SelectionItem(new Selection(1, Function.DefendWithUnit, true, SelectionCondition.alliedUntappedUnit), "Choose a unit to defend with");

        }
        public override void OnPress()
        {
            if (canBePressed)
                Primary.game.currentForm = new SelectionForm((InGameForm)Primary.game.currentForm, selection.Value);

        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.DrawString(Primary.game.mainFont, buttonText, new Vector2(boundingBox.X, boundingBox.Y), canBePressed ? Color.Black : Color.White);
        }
        public override void Update()
        {
            base.Update();
        }
    }
    /// <summary>
    /// Changes the current form to SelectionForm. Only used for countering enemy card plays.
    /// </summary>
    class IGCounterButton : TexturedButton
    {
        List<SmallCard> cards = new List<SmallCard>();
        Function function;
        public IGCounterButton(Rectangle r, string text, List<SmallCard> c, Function f) : base(r, Primary.game.buttonTexture)
        {
            function = f;
            buttonText = text;
            cards = c;
            if (c.Count != 0)
                canBePressed = true;
            else canBePressed = false;
        }
        public override void OnPress()
        {
            if (canBePressed)
                Primary.game.currentForm = new SelectionForm((InGameForm)Primary.game.currentForm, cards, function);

        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            sb.DrawString(Primary.game.mainFont, buttonText, new Vector2(boundingBox.X, boundingBox.Y), canBePressed ? Color.Black : Color.White);
        }
        public override void Update()
        {
            base.Update();
        }
    }
    /// <summary>
    /// Button to make no selection if the user is offered a selection
    /// </summary>
    class IGCancelButton : Button
    {
        public IGCancelButton(Rectangle r) : base(r, "Make no selection")
        {
            boundingBox = r;
        }

        public override void OnPress()
        {
#warning need to change this depending on whether the end item of the chain was player played or not
            ((InGameForm)Primary.game.currentForm).ResolveChain();
            Primary.game.SendData(new byte[] { (byte)Protocol.NoCounter });
        }
        public override void Update()
        {
            base.Update();
        }
    }
    /// <summary>
    /// Accept the selection in the SelectionForm
    /// </summary>
    class sSelectButton : Button
    {
        bool pressable;
        public sSelectButton(Rectangle r) : base(r, "Make Selection")
        {
            pressable = false;
        }
        public override void Update()
        {
            try
            {
                pressable = ((SelectionForm)Primary.game.currentForm).bigCard != null;
            }
            catch { }
            base.Update();
        }
        public override void OnPress()
        {
            if (pressable)
            {
                SelectionForm currentForm = (SelectionForm)Primary.game.currentForm;
                currentForm.gameForm.HandleSelection(currentForm.GetSelectedCard(), currentForm.function);

                //currentForm.gameForm.chain.RemoveLast();
                //currentForm.gameForm.ResolveChain();
                Primary.game.currentForm = currentForm.gameForm;

            }
        }
    }
    /// <summary>
    /// Return to the InGameForm from the SelectionForm
    /// </summary>
    class sBackButton : Button
    {
        public sBackButton(Rectangle r) : base(r, "Back") { }
        public override void OnPress()
        {
            Primary.game.currentForm = ((SelectionForm)Primary.game.currentForm).gameForm;
        }
    }
}
