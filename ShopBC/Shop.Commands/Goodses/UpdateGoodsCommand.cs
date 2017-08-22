using System;
using ENode.Commanding;
using System.Collections.Generic;

namespace Shop.Commands.Goodses
{
    public class UpdateGoodsCommand : Command<Guid>
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IList<string> Pics { get; private set; }
        public decimal Price { get; private set; }
        public int SellOut { get; private set; }

        public UpdateGoodsCommand() { }

        public UpdateGoodsCommand(
            string name,
            string description,
            IList<string> pics,
            decimal price,
            int sellOut)
        {
            Name = name;
            Description = description;
            Pics = pics;
            Price = price;
            SellOut = sellOut;
        }
    }
}
