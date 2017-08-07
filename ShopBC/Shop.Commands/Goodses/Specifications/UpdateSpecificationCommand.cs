﻿using ENode.Commanding;
using System;

namespace Shop.Commands.Goodses.Specifications
{
    public class UpdateSpecificationCommand:Command<Guid>
    {
        public Guid SpecificationId { get;private set; }
        public string Name { get; private set; }
        public string Thumb { get; private set; }
        public decimal Price { get; private set; }
        public decimal OriginalPrice { get; private set; }
        public string Number { get; private set; }
        public string BarCode { get; private set; }
        public int Stock { get; private set; }

        public UpdateSpecificationCommand() { }
        public UpdateSpecificationCommand(Guid goodsId,
            Guid specificationId,
            string name,
            string thumb,
            decimal price,
            decimal originalPrice,
            string number,
            string barCode,
            int stock):base(goodsId)
        {
            SpecificationId = specificationId;
            Name = name;
            Thumb = thumb;
            Price = price;
            OriginalPrice = originalPrice;
            Number = number;
            BarCode = barCode;
            Stock = stock;
        }

    }
}