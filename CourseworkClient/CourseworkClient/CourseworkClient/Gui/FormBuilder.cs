using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient.Gui
{
    enum FormChangeButtonTypes
    {
        LogInToCreateAccount,
        CreateAccountToLogIn
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
            }
            throw new ArgumentException("Something's gone very wrong here");
        }
    }
}
