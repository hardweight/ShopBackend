using System;
using System.Collections.Generic;
using Shop.ReadModel.Goodses.Dtos;

namespace Shop.ReadModel.Goodses
{
    /// <summary>
    /// 产品查询服务接口
    /// </summary>
    public interface IGoodsQueryService
    {
        GoodsDetails GetGoodsDetails(Guid goodsId);
        GoodsAlias GetGoodsAlias(Guid goodsId);

        IList<GoodsAlias> GetPublishedGoodses();
        IList<Specification> GetPublishedSpecifications(Guid goodsId);
        IList<SpecificationName> GetSpecificationNames(IEnumerable<Guid> specifications);
    }
}