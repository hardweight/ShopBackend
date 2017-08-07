﻿using System;
using ENode.Commanding;

namespace Shop.Commands.Goodses.Specifications
{
    public class AddSpecificationCommand : Command<Guid>
    {
        public string Name { get; private set; }
        public string Thumb { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public decimal OriginalPrice { get; private set; }
        public string Number { get; private set; }
        public string BarCode { get; private set; }

        public AddSpecificationCommand() { }
        public AddSpecificationCommand(
            Guid goodsId,
            string name,
            string thumb,
            decimal price,
            int stock,
            decimal originalPrice,
            string number,
            string barCode
            ) : base(goodsId)
        {
            Name = name;
            Thumb = thumb;
            Price = price;
            Stock = stock;
            OriginalPrice = originalPrice;
            Number = number;
            BarCode = barCode;
        }
    }
}
