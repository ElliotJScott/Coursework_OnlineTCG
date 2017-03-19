using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseworkServer
{
    /// <summary>
    /// The priorities for the queue
    /// </summary>
    enum Priority
    {
        standard,
        high
    }
    /// <summary>
    /// The different operations that can be done to the items in the queue
    /// </summary>
    public enum Operation
    {
        AddNewAccount,
        CheckCredentials,
        AddToQueue,
        GetPlayerEloAndCoin,
        TransmitDBData,
        CalculateEloCoinChanges,
    }
    class ActionItem
    {
        public Priority priority;
        public Operation operation;
        public object data;
        public Client sender;
        /// <summary>
        /// Constructor for an ActionItem, which is the items that are put into the queue to be processed
        /// </summary>
        /// <param name="op">The operation that needs to be done to the data</param>
        /// <param name="d">The data that needs to be processed</param>
        /// <param name="c">The client that sent the data</param>
        /// <param name="p">The priority of the item in the queue. Default value is standard priority.</param>
        public ActionItem(Operation op, object d, Client c, Priority p = Priority.standard)
        {
            sender = c;
            operation = op;
            data = d;
            priority = p;
        }
    }
}
