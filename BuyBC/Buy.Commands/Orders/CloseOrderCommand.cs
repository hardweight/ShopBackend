using System;
using ENode.Commanding;

namespace Buy.Commands.Orders
{
    [Serializable]
    public class CloseOrderCommand : Command<Guid>
    {
        public Guid GoodsId { get; private set; }

        public CloseOrderCommand() { }
        public CloseOrderCommand(Guid orderId,Guid goodsId) : base(orderId)
        {
            GoodsId = goodsId;
        }
    }
}
