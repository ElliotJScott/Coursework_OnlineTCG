
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
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
        MainMenuToDeckManager
    }
    class FormBuilder
    {
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
                case FormChangeButtonTypes.OptionsToMainMenu:
                case FormChangeButtonTypes.QueueSelectToMainMenu:
                case FormChangeButtonTypes.StoreToMainMenu:
                    return new MainMenuForm();
                    
                default:
                    System.Windows.Forms.MessageBox.Show("Uh-oh. The client just tried to go to a form that doesn't exist. That's not good");
                    Primary.game.ExitGame();
                    throw new Exception();
                    
            }
            
        }
    }
}
