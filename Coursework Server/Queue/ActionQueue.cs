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
                    Execute(a);
                    items.RemoveAt(0);

                }
                else if (items.Count != 0)
                {
                    ActionItem a = items.ElementAt(0);
                    Execute(a);
                    items.RemoveAt(0);
                    
                }
            }
        }
        public void Execute(ActionItem i)
        {
            Executor e = new Executor();
            e.currentItem = i;
            Thread t = new Thread(new ThreadStart(e.Execute));
            t.Start();
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
