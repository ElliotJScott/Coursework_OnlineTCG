using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
    enum Priority //If needed add a third priority for really important things and maybe a low priority for unimportant things
    {
        standard,
        high
    }
    public enum Operation
    {
        AddNewAccount,
        CheckCredentials,
        CheckFriendStatus
    }
    class ActionItem
    {
        public Priority priority;
        public Operation operation;
        public object data;
        public Client sender;
        public ActionItem(Operation op, object d, Client c, Priority p = Priority.standard)
        {
            sender = c;
            operation = op;
            data = d;
            priority = p;
        }
        public override string ToString()
        {
            return string.Format("ActionItem: Priority = %s | Operation = %s | Data = %d", priority.ToString(), operation.ToString(), data); 
        }
    }
}
