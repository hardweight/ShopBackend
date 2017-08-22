using Autofac;
using ECommon.Autofac;
using ECommon.Components;
using ENode.Commanding;
using Quartz;
using Shop.Commands.Orders;
using Shop.ReadModel.Orders;
using System.Diagnostics;
using System.Linq;

namespace Shop.TimerTask.Jobs.Orders
{
    /// <summary>
    /// 预订单任务
    /// </summary>
    public class ExpiredOrderJob : IJob
    {
        private ICommandService _commandService;//C端
        private OrderQueryService _orderQueryService;//Q 端

        public ExpiredOrderJob()
        {
            var container = (ObjectContainer.Current as AutofacObjectContainer).Container;
            _commandService = container.Resolve<ICommandService>();
            _orderQueryService = container.Resolve<OrderQueryService>();
        }
        
        /// <summary>
        /// 计划任务
        /// </summary>
        /// <param name="context"></param>
        public  void Execute(IJobExecutionContext context)
        {
            Debug.WriteLine("任务执行了");
            ProcessExpiredOrder();
        }


        private void ProcessExpiredOrder()
        {
            //获取所有过期未支付的预订单
            var expiredUnPayOrders = _orderQueryService.ExpiredUnPayOrders();
            if (expiredUnPayOrders.Any())
            {
                foreach (var expiredOrder in expiredUnPayOrders)
                {
                    var command = new MarkAsExpiredCommand(expiredOrder.OrderId);
                    _commandService.SendAsync(command);
                }
            }
        }
    }
}