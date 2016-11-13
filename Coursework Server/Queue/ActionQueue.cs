using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            Thread t = new Thread(new ThreadStart(BeginDelegating));
            t.Start();
        }
        public void BeginDelegating()
        {
            while (true)
            {
                if (priorityItems.Count != 0)
                {
                    ActionItem a = priorityItems.ElementAt(0);
                    if (!Server.server.delegator.Delegate(a))
                    {
                        priorityItems.RemoveAt(0);
                    }
                    else Console.WriteLine("No threads available to delegate to. Consider adding more threads.");
                }
                else
                {
                    ActionItem a = items.ElementAt(0);
                    if (!Server.server.delegator.Delegate(a))
                    {
                        items.RemoveAt(0);
                    }
                    else
                    {
                        Console.WriteLine("No threads available to delegate to. Consider adding more threads.");
                        Console.WriteLine("Upgrading priority of current item");
                        items.RemoveAt(0);
                        priorityItems.Add(a);
                    }
                }
            }
        }
        public void Enqueue(ActionItem a)
        {
            switch (a.priority)
            {
                case Priority.high:
                    priorityItems.Add(a);
                    return;
                case Priority.standard:
                    items.Add(a);
                    return;
                default:
                    throw new ArgumentException("Something's gone very wrong here: " + a.ToString());                    
            }
        }
    }
}
