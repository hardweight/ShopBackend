using Shop.ReadModel.PubCategorys.Dtos;
using System;
using System.Collections.Generic;

namespace Shop.ReadModel.PubCategorys
{
    /// <summary>
    /// 查询服务接口
    /// </summary>
    public interface IPubCategoryQueryService
    {
        PubCategory Find(Guid id);

        IEnumerable<PubCategory> RootCategorys();

        IEnumerable<PubCategory> GetChildren(Guid id);

    }
}