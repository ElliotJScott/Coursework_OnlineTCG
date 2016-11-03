using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourseworkServer
{
    class Delegator
    {
        int[] executorUsage = new int[Server.executionThreads];
        public Delegator()
        {
            for (int i = 0; i < Server.executionThreads; i++)
            {
                executorUsage[i] = 0;
            }
            Console.WriteLine("Creating delegator");
        }
        public bool Delegate(ActionItem a)
        {
            for (int i = 0; i < Server.executionThreads; i++)
            {
                Executor e = Server.server.executors[i];
                if (e.occupied == false)
                {
                    e.currentItem = a;
                    Thread t = new Thread(new ThreadStart(e.Execute));
                    t.Start();
                    Console.WriteLine("Successfully delegated ActionItem " + a.ToString());
                    return true;
                }
                executorUsage[i]++;
            }
            Console.WriteLine("Failed to delegate ActionItem " + a.ToString());
            return false;
        }
    }
}
