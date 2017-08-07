using Buy.Domain.Orders.Events;
using Buy.Domain.Orders.Models;
using ECommon.Components;
using ECommon.Dapper;
using ECommon.IO;
using ENode.Infrastructure;
using Shop.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buy.ReadModel
{
    [Component]
    public class OrderViewModelGenerator : BaseGenerator,
        IMessageHandler<OrderPlacedEvent>,
        IMessageHandler<OrderReservationConfirmedEvent>,
        IMessageHandler<OrderPaymentConfirmedEvent>,
        IMessageHandler<OrderExpiredEvent>,
        IMessageHandler<OrderClosedEvent>,
        IMessageHandler<OrderSuccessedEvent>
    {
        public Task<AsyncTaskResult> HandleAsync(OrderPlacedEvent evnt)
        {
            return TryTransactionAsync((connection, transaction) =>
            {
                var tasks = new List<Task>();

                //插入订单主记录
                tasks.Add(connection.InsertAsync(new
                {
                    OrderId = evnt.AggregateRootId,
                    UserId=evnt.UserId,
                    Status = (int)OrderStatus.Placed,
                    ReservationExpirationDate = evnt.ReservationExpirationDate,
                    TotalAmount = evnt.OrderTotal.Total,
                    Version = evnt.Version
                }, ConfigSettings.OrderTable, transaction));

                //插入订单明细
                foreach (var line in evnt.OrderTotal.Lines)
                {
                    tasks.Add(connection.InsertAsync(new
                    {
                        OrderId = evnt.AggregateRootId,
                        SpecificationId = line.SpecificationQuantity.Specification.SpecificationId,
                        SpecificationName = line.SpecificationQuantity.Specification.SpecificationName,
                        Quantity = line.SpecificationQuantity.Quantity,
                        UnitPrice = line.SpecificationQuantity.Specification.UnitPrice,
                        LineTotal = line.LineTotal
                    }, ConfigSettings.OrderLineTable, transaction));
                }
                return tasks;
            });
        }
        
        public Task<AsyncTaskResult> HandleAsync(OrderReservationConfirmedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Status = (int)evnt.OrderStatus,
                    Version = evnt.Version
                }, new
                {
                    OrderId = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.OrderTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(OrderPaymentConfirmedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Status = (int)evnt.OrderStatus,
                    Version = evnt.Version
                }, new
                {
                    OrderId = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.OrderTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(OrderExpiredEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Status = (int)OrderStatus.Expired,
                    Version = evnt.Version
                }, new
                {
                    OrderId = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.OrderTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(OrderClosedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Status = (int)OrderStatus.Closed,
                    Version = evnt.Version
                }, new
                {
                    OrderId = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.OrderTable);
            });
        }
        public Task<AsyncTaskResult> HandleAsync(OrderSuccessedEvent evnt)
        {
            return TryUpdateRecordAsync(connection =>
            {
                return connection.UpdateAsync(new
                {
                    Status = (int)OrderStatus.Success,
                    Version = evnt.Version
                }, new
                {
                    OrderId = evnt.AggregateRootId,
                    Version = evnt.Version - 1
                }, ConfigSettings.OrderTable);
            });
        }

    }
}
