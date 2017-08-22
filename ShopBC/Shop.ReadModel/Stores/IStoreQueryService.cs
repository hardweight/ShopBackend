using Shop.ReadModel.Stores.Dtos;
using System;
using System.Collections.Generic;

namespace Shop.ReadModel.Stores
{
    /// <summary>
    /// 店铺查询接口
    /// </summary>
    public interface  IStoreQueryService
    {
        Store Info(Guid id);

        IEnumerable<Store> StoreList();

        Store InfoByUserId(Guid userId);

        decimal TodaySale();
    }
}
