using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
    class ActionQueue
    {       
        List<ActionItem> items = new List<ActionItem>();
        List<ActionItem> priorityItems = new List<ActionItem>();
        public ActionQueue()
        {
            Console.WriteLine("Queue set up");
            BeginDelegating();
        }
        public void BeginDelegating()
        {
            while (true)
            {
                if (priorityItems.Count != 0)
                {
                    ActionItem a = priorityItems.ElementAt(0);
                    bool b = Server.
                }
            }
        }
    }
}
