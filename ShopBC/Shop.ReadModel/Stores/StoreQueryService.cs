using ECommon.Components;
using ECommon.Dapper;
using Shop.Common;
using System;
using System.Linq;

namespace Shop.ReadModel.Stores
{
    [Component]
    public class StoreQueryService: BaseQueryService,IStoreQueryService
    {
        public Dtos.Store Info(Guid id)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Dtos.Store>(new { Id = id }, ConfigSettings.StoreTable).SingleOrDefault();
            }
        }
        /// <summary>
        /// 钱包信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Dtos.Store InfoByUserId(Guid userId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Dtos.Store>(new { UserId = userId }, ConfigSettings.StoreTable).SingleOrDefault();
            }
        }
    }
}
