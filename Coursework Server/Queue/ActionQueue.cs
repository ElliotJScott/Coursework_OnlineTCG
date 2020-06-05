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

        List<ActionItem> items = new List<ActionItem>(); //The standard priority item queue
        List<ActionItem> priorityItems = new List<ActionItem>(); //The high priority item queue
        /// <summary>
        /// Constructor to set up the queue. The queue operates entirely in its own thread.
        /// </summary>
        public ActionQueue()
        {
            Thread t = new Thread(new ThreadStart(BeginDelegating));
            t.Start();
        }
        /// <summary>
        /// Begins executing all of the items in the queue in order.
        /// </summary>
        public void BeginDelegating()
        {
            while (true)
            {
                if (priorityItems.Count != 0)
                {
                    ActionItem a = priorityItems.ElementAt(0);
                    Execute(a);
                    priorityItems.RemoveAt(0);

                }
                else if (items.Count != 0)
                {
                    ActionItem a = items.ElementAt(0);
                    Execute(a);
                    items.RemoveAt(0);
                    
                }
            }
        }
        /// <summary>
        /// Executes the given ActionItem in a new thread.
        /// </summary>
        /// <param name="i">The ActionItem to execute</param>
        public void Execute(ActionItem i)
        {
            Executor e = new Executor();
            e.currentItem = i;
            Thread t = new Thread(new ThreadStart(e.Execute));
            t.Start();
        }
        /// <summary>
        /// Adds an item to the end of the correct queue
        /// </summary>
        /// <param name="a">The item to add to the queue. The priority is stored in the item.</param>
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
