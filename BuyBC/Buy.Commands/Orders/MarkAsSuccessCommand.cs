using System;
using ENode.Commanding;

namespace Buy.Commands.Orders
{
    [Serializable]
    public class MarkAsSuccessCommand : Command<Guid>
    {
        public Guid GoodsId { get; private set; }

        public MarkAsSuccessCommand() { }
        public MarkAsSuccessCommand(Guid orderId,Guid goodsId) : base(orderId)
        {
            GoodsId = goodsId;
        }
    }
}
