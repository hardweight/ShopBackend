using ECommon.Components;
using ECommon.Dapper;
using Shop.Common;
using Shop.ReadModel.Stores.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Shop.ReadModel.Stores
{
    [Component]
    public class StoreQueryService: BaseQueryService,IStoreQueryService
    {
        public Store Info(Guid id)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Store>(new { Id = id }, ConfigSettings.StoreTable).SingleOrDefault();
            }
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Store InfoByUserId(Guid userId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Store>(new { UserId = userId }, ConfigSettings.StoreTable).SingleOrDefault();
            }
        }

        /// <summary>
        /// 今日所有商家销售额
        /// </summary>
        /// <returns></returns>
        public decimal TodaySale()
        {
            //var sql = string.Format(@"select sum(TodaySale) as totaltodaySale from {0}", ConfigSettings.StoreTable);

            //using (var connection = GetConnection())
            //{
            //    return connection.Query<decimal>(sql).FirstOrDefault();
            //}
            using (var connection = GetConnection())
            {
                var stores= connection.QueryList<Store>(new { }, ConfigSettings.StoreTable);
                return stores.Sum(x => x.TodaySale);
            }
        }

        public IEnumerable<Store> StoreList()
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Store>(new { }, ConfigSettings.StoreTable);
            }
        }
    }
}
