using ECommon.Components;
using ECommon.Dapper;
using Shop.Common;
using Shop.ReadModel.Carts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Shop.ReadModel.Carts
{
    /// <summary>
    /// 查询服务 实现
    /// </summary>
    [Component]
    public class CartQueryService : BaseQueryService, ICartQueryService
    {
        public Cart Info(Guid id)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Cart>(new { Id = id }, ConfigSettings.CartTable).SingleOrDefault();
            }
        }

        /// <summary>
        /// 获取购物车中的商品
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        public IEnumerable<CartGoods> CartGoodses(Guid cartId)
        {
            var sql = string.Format(@"select a.Id,
            a.StoreId,
            a.GoodsId,
            a.SpecificationId,
            a.GoodsName,
            a.GoodsPic,
            a.SpecificationName,
            a.Price,
            a.OriginalPrice,
            a.Quantity,
            a.Stock,
            b.Name as StoreName
            from {0} a inner join {1} b on a.StoreId=b.Id 
            where a.CartId='{2}'", ConfigSettings.CartGoodsesTable, ConfigSettings.StoreTable,cartId);

            using (var connection = GetConnection())
            {
                return connection.Query<CartGoods>(sql);
            }
        }


    }
}