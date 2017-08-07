using System;

namespace Shop.ReadModel.Stores
{
    /// <summary>
    /// 店铺查询接口
    /// </summary>
    public interface  IStoreQueryService
    {
        Dtos.Store Info(Guid id);

        Dtos.Store InfoByUserId(Guid userId);
    }
}
