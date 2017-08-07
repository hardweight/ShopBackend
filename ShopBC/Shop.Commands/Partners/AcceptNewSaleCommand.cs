using ENode.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Commands.Partners
{
    public class AcceptNewSaleCommand:Command<Guid>
    {
        public string Region { get; private set; }
        public decimal Amount { get; private set; }

        public AcceptNewSaleCommand() { }
        public AcceptNewSaleCommand(string region,decimal amount)
        {
            Region = region;
            Amount = amount;
        }
    }
}
