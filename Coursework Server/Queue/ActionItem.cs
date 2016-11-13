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
    class ActionItem
    {
        public Priority priority;
        int operation;
        object data;
        public ActionItem(int op, object d, Priority p = Priority.standard)
        {
            operation = op;
            data = d;
            priority = p;
        }
        public override string ToString()
        {
            return String.Format("ActionItem: Priority = %s | Operation = %d | Data = %d", priority.ToString(), operation, data); 
        }
    }
}
