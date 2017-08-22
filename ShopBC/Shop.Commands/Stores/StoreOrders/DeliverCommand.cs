using ENode.Commanding;
using System;

namespace Shop.Commands.Stores.StoreOrders
{
    public class DeliverCommand:Command<Guid>
    {
        public string ExpressName { get; private set; }
        public string ExpressNumber { get; private set; }

        public DeliverCommand() { }
        public DeliverCommand(string expressName,string expressNumber)
        {
            ExpressName = expressName;
            ExpressNumber = expressNumber;
        }
    }
}
