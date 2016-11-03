using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
    class ActionItem
    {
        int priority;
        int operand;
        byte[] data = new byte[128]; //Probably change this to object or something
    }
}
