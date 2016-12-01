using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    public enum Protocol
    {
        Connect,
        Disconnect,
        LogIn,
        CreateAccount,
        BadCredentials,
        GoodCredentials,
        UsernameTaken

    }
}
