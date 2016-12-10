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
        InQueue,
        InGame
    }
   
    public class Friend
    {
        public string userName;
        public Status status;
        public List<string> chatMessages = new List<string>();
        public bool Online() => !(status == Status.Offline);
        public Friend(string u, Status s = Status.Offline)
        {
            userName = u;
            status = s;

        }
        public static bool operator ==(Friend a, Friend b)
        {
            return a.userName == b.userName;
        }
        public static bool operator !=(Friend a, Friend b)
        {
            return !(a == b);
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            else if (this == null || obj == null) return false;
            else return (Friend)obj == this;
        }
    }
    
}
