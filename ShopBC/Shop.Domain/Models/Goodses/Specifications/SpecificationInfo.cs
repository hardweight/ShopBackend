using System;

namespace Shop.Domain.Models.Goodses.Specifications
{
    /// <summary>
    /// 商品规格基本信息
    /// </summary>
    [Serializable]
    public class SpecificationInfo
    {
        /// <summary>
        /// 规格名称 红色+M
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 缩略图
        /// </summary>
        public string Thumb { get; private set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; private set; }
        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; private set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string Number { get; private set; }
        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; private set; }

        public SpecificationInfo(string name, string thumb, decimal price,decimal originalPrice,string number,string barCode)
        {
            Name = name;
            Thumb = thumb;
            Price = price;
            OriginalPrice = originalPrice;
            Number = number;
            BarCode = barCode;
        }
    }
}
