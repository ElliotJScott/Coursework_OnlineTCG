using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
    class Executor
    {
        public bool occupied;
        public ActionItem currentItem = null;
        public Executor()
        {
            Console.WriteLine("Creating executor");
            occupied = false;
        }
        public void Execute() 
        {
        }
    }
}
