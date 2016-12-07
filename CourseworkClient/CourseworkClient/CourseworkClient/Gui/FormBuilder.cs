
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
                    throw new ArgumentException("Something's gone very wrong here");
            }
            
        }
    }
}
