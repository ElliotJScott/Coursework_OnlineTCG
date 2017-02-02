
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    /// <summary>
    /// The different transitions that a FormChangeButton can make
    /// </summary>
    enum FormChangeButtonTypes
    {
        LogInToCreateAccount,
        CreateAccountToLogIn,
        MainMenuToOptions,
        OptionsToMainMenu,
        MainMenuToQueueSelect,
        QueueSelectToMainMenu,
        MainMenuToStore,
        StoreToMainMenu,
        MainMenuToDeckManager,
        DeckManagerToMainMenu,

    }
    class FormBuilder
    {
        /// <summary>
        /// Gets a new form from a transition
        /// </summary>
        /// <param name="type">The form transition to make</param>
        /// <returns>The form to transition to</returns>
        public static Form BuildNewForm(FormChangeButtonTypes type)
        {
            switch (type)
            {
                case FormChangeButtonTypes.CreateAccountToLogIn:
                    return new LoginScreenForm();
                case FormChangeButtonTypes.LogInToCreateAccount:
                    return new CreateAccountForm();
                case FormChangeButtonTypes.MainMenuToOptions:
                    return new OptionsMenuForm();
                case FormChangeButtonTypes.MainMenuToQueueSelect:
                    return new QueueSelectForm();
                case FormChangeButtonTypes.MainMenuToDeckManager:
#warning This will most certainly need to be changed
                    return new InGameForm(Deck.decks[Primary.game.selectedDeckNum], true, "Test");
                case FormChangeButtonTypes.OptionsToMainMenu:
                case FormChangeButtonTypes.QueueSelectToMainMenu:
                case FormChangeButtonTypes.StoreToMainMenu:
                    return new MainMenuForm();
                    
                default:
                    System.Windows.Forms.MessageBox.Show("Uh-oh. The client just tried to go to a form that doesn't exist. That's not good");
                    Primary.game.ExitGame();
                    throw new Exception(); //This line won't do anything because the program will exit before it is called
                    
            }
            
        }
    }
}
