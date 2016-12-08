﻿namespace CourseworkServer
{
    public enum Protocol
    {
        Connect,
        Disconnect,
        LogIn,
        CreateAccount,
        BadCredentials,
        GoodCredentials,
        UsernameTaken,
        AddToQueue,
        FriendStatus,
        LoggedIn
    }
}
