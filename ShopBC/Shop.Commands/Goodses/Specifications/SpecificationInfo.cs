using System;

namespace Shop.Commands.Goodses.Specifications
{
    [Serializable]
    public class SpecificationInfo
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Thumb { get; private set; }
        public decimal Price { get; private set; }
        public decimal OriginalPrice { get; private set; }
        public string Number { get; private set; }
        public string BarCode { get; private set; }
        public int Stock { get; private set; }

        public SpecificationInfo(string name,
            string value,
            string thumb,
            decimal price,
            decimal originalPrice,
            string number,
            string barCode,
            int stock)
        {
            Name = name;
            Value = value;
            Thumb = thumb;
            Price = price;
            OriginalPrice = originalPrice;
            Number = number;
            BarCode = barCode;
            Stock = stock;
        }
    }
}
