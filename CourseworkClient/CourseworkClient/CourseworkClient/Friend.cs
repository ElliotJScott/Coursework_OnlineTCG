using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    public enum Status
    {
        Offline,
        Online,
        InGame
    }
   
    class Friend
    {
        string userName;
        public Status status;
        List<string> chatMessages = new List<string>();
        public bool Online() => !(status == Status.Offline);
    
    }
    
}
