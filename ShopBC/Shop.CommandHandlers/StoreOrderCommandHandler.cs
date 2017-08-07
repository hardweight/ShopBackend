using ECommon.Components;
using ENode.Commanding;
using ENode.Infrastructure;
using Shop.Commands.Stores.StoreOrders;
using Shop.Domain.Models.Stores;
using System;
using System.Linq;

namespace Shop.CommandHandlers
{
    /// <summary>
    /// 领域模型 事件处理
    /// </summary>
    [Component]
    public class StoreOrderCommandHandler:
        ICommandHandler<CreateStoreOrderCommand>
    {
        private readonly ILockService _lockService;

        /// <summary>
        /// IOC 构造函数注入
        /// </summary>
        /// <param name="lockService"></param>
        public StoreOrderCommandHandler(ILockService lockService)
        {
            _lockService = lockService;
        }

        #region handle Command

      
        
        public void Handle(ICommandContext context,CreateStoreOrderCommand command)
        {
            //从上下文中获取商家的地区
            var region = context.Get<Store>(command.StoreId).GetInfo().Region;
            var storeOrder = new StoreOrder(
                    command.AggregateRootId,
                    command.OrderId,
                    command.StoreId,
                    region,
                    command.OrderGoodses.Select(x => new OrderGoodsInfo(
                        x.GoodsId,
                        x.SpecificationId,
                        x.SpecificationName,
                        x.SpecificationName,
                        0,
                        x.Quantity,
                        x.Total,
                        DateTime.Now,
                        x.SurrenderPersent)
                    ).ToList()
                );
            //添加到上下文
            context.Add(storeOrder);
        }

        #endregion
    }
}
