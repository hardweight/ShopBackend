using ECommon.Components;
using ECommon.Dapper;
using Shop.Common;
using Shop.ReadModel.Goodses.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.ReadModel.Goodses
{
    /// <summary>
    /// 产品查询服务 实现
    /// </summary>
    [Component]
    public class GoodsQueryService : BaseQueryService,IGoodsQueryService
    {
        public GoodsDetails GetGoodsDetails(Guid goodsId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<GoodsDetails>(new { Id = goodsId }, ConfigSettings.GoodsTable).SingleOrDefault();
            }
        }
        public GoodsAlias GetGoodsAlias(Guid goodsId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<GoodsAlias>(new { Id = goodsId }, ConfigSettings.GoodsTable).SingleOrDefault();
            }
        }

        public IList<GoodsAlias> GetPublishedGoodses()
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<GoodsAlias>(new { IsPublished = 1 }, ConfigSettings.GoodsTable).ToList();
            }
        }

        public IList<Specification> GetPublishedSpecifications(Guid goodsId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Specification>(new { GoodsId = goodsId }, ConfigSettings.SpecificationTable).ToList();
            }
        }
        public IList<SpecificationName> GetSpecificationNames(IEnumerable<Guid> specifications)
        {
            var distinctIds = specifications.Distinct().ToArray();
            if (distinctIds.Length == 0)
            {
                return new List<SpecificationName>();
            }

            using (var connection = GetConnection())
            {
                var result = new List<SpecificationName>();
                foreach (var specificationId in distinctIds)
                {
                    var specification = connection.QueryList<Specification>(new { Id = specificationId }, ConfigSettings.SpecificationTable).SingleOrDefault();
                    if (specification != null)
                    {
                        result.Add(new SpecificationName { Id = specification.Id, Name = specification.Name });
                    }
                }
                return result;
            }
        }

       

    }
}