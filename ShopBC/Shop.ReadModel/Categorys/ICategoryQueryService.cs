using Shop.ReadModel.Categorys.Dtos;
using System;
using System.Collections.Generic;

namespace Shop.ReadModel.Categorys
{
    /// <summary>
    /// 查询服务接口
    /// </summary>
    public interface ICategoryQueryService
    {
        IEnumerable<Category> RootCategorys();

        IEnumerable<Category> GetChildren(Guid id);

    }
}