﻿using System;

namespace Shop.Domain.Models.Carts.CartGoods
{
    public class CartGoodsInfo
    {
        public Guid StoreId { get; private set; }
        public Guid GoodsId { get;private set; }
        public Guid SpecificationId { get; private set; }
        public string GoodsName { get; private set; }
        public string SpecificationName { get;private set; }
        public decimal Price { get; private set; }
        public int Quantity { get;  set; }
        public int Stock { get;private set; }

        public CartGoodsInfo(
            Guid storeId,
            Guid goodsId,
            Guid specificationId,
            string goodsName,
            string specificationName,
            decimal price,
            int quantity,
            int stock)
        {
            StoreId = storeId;
            GoodsId = goodsId;
            SpecificationId = specificationId;
            GoodsName = goodsName;
            SpecificationName = specificationName;
            Price = price;
            Quantity = quantity;
            Stock = stock;
        }
    }
}