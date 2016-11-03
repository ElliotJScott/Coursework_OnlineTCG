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
        int operand;
        object data;
        public ActionItem(int op, object d, Priority p = Priority.standard)
        {
            operand = op;
            data = d;
            priority = p;
        }
    }
}
