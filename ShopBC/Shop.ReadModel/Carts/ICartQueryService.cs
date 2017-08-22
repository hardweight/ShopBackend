using Shop.ReadModel.Carts.Dtos;
using System;
using System.Collections.Generic;

namespace Shop.ReadModel.Carts
{
    /// <summary>
    /// 查询服务接口
    /// </summary>
    public interface ICartQueryService
    {
        Cart Info(Guid id);

        IEnumerable<CartGoods> CartGoodses(Guid cartId);
    }
}