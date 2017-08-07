using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ENode.Commanding;

namespace Buy.Commands.Orders
{
    [Serializable]
    public class PlaceOrderCommand : Command<Guid>
    {
        public Guid UserId { get;private set; }
        public IList<SpecificationiInfo> Specifications { get;private set; }

        public PlaceOrderCommand() { }
        public PlaceOrderCommand(Guid id,Guid userId) : base(id)
        {
            UserId = userId;
            Specifications = new Collection<SpecificationiInfo>();
        }
    }
}
