
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
        MainMenuToQueue,
        QueueToMainMenu,
        MainMenuToStore,
        StoreToMainMenu,
        MainMenuToDeckManager,
        DeckManagerToMainMenu,
        EndGameToMainMenu,
        PackOpeningToMainMenu,
        LeaderboardToMainMenu,
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
                case FormChangeButtonTypes.MainMenuToQueue:
                    Primary.game.WriteDataToStream(Protocol.AddToQueue);
                    return new QueueForm();
                case FormChangeButtonTypes.MainMenuToDeckManager:
                    return new DeckManagerForm();
                case FormChangeButtonTypes.MainMenuToStore:
                    return new StoreForm();
                case FormChangeButtonTypes.QueueToMainMenu:
                    Primary.game.WriteDataToStream(Protocol.RemoveFromQueue);
                    return new MainMenuForm();
                case FormChangeButtonTypes.OptionsToMainMenu:
                case FormChangeButtonTypes.StoreToMainMenu:
                case FormChangeButtonTypes.EndGameToMainMenu:
                case FormChangeButtonTypes.PackOpeningToMainMenu:
                case FormChangeButtonTypes.DeckManagerToMainMenu:
                case FormChangeButtonTypes.LeaderboardToMainMenu:
                    return new MainMenuForm();
                    
                default:
                    System.Windows.Forms.MessageBox.Show("Uh-oh. The client just tried to go to a form that doesn't exist. That's not good");
                    Primary.game.ExitGame();
                    throw new Exception(); //This line won't do anything because the program will exit before it is called
                    
            }
            
        }
    }
}
