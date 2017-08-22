using ECommon.Utilities;
using ENode.Domain;
using Shop.Common;
using Shop.Common.Enums;
using Shop.Domain.Events.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Domain.Models.Orders
{
    /// <summary>
    /// 预订单
    /// </summary>
    public class Order: AggregateRoot<Guid>
    {
        private Guid _userId;//订单人
        private OrderTotal _total;//订单小计
        private OrderStatus _status;//订单状态
        private Guid _registrantId;//预定人
        private ExpressAddressInfo _expressAddressInfo;//收货地址
        private IDictionary<Guid, bool> _specificationReservationStatus;//商品的预定状态
        private IList<Guid> _specificationConfirmStatus;//保存商品的确认状态
        private DateTime _reservationExpirationDate;//预定过期时间

        private static readonly TimeSpan WaitTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(2);//每半小时轮询付款状态


        public Order(Guid id,
            Guid userId, 
            ExpressAddressInfo expressAddressInfo,
            IEnumerable<SpecificationQuantity> specifications,
            IPricingService pricingService) : base(id)
        {
            Ensure.NotEmptyGuid(id, nameof(id));
            Ensure.NotNull(specifications, nameof(specifications));
            Ensure.NotNull(pricingService, nameof(pricingService));

            if (!specifications.Any()) throw new ArgumentException("订单的商品不能为空");

            var orderTotal = pricingService.CalculateTotal(specifications);
            ApplyEvent(new OrderPlacedEvent(userId,
                expressAddressInfo,
                orderTotal,
                DateTime.Now.Add(ConfigSettings.ReservationAutoExpiration)));
        }

        /// <summary>
        /// 某一个商品确认预定
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="isReservationSuccess">成功与否</param>
        public void ConfirmOneReservation(Guid goodsId,bool isReservationSuccess)
        {
            if(_specificationReservationStatus==null)
            {
                _specificationReservationStatus= new Dictionary<Guid,bool>();
            }
            if(!_specificationReservationStatus.ContainsKey(goodsId))
            {
                _specificationReservationStatus.Add(goodsId, isReservationSuccess);
            }

            //判断是否所有商品都反馈了预定信息
            if(_specificationReservationStatus.Count==_total.Lines.Length)
            {
                ConfirmReservation(!_specificationReservationStatus.Any(k => k.Value == false));
            }
        }
        /// <summary>
        /// 确认订单支付
        /// </summary>
        /// <param name="isPaymentSuccess"></param>
        public void ConfirmPayment(bool isPaymentSuccess)
        {
            if (_status != OrderStatus.ReservationSuccess)
            {
                throw new InvalidOperationException("不正确的订单状态:" + _status);
            }
            if (isPaymentSuccess)
            {
                ApplyEvent(new OrderPaymentConfirmedEvent(_total, OrderStatus.PaymentSuccess));
            }
            else
            {
                ApplyEvent(new OrderPaymentConfirmedEvent(_total,OrderStatus.PaymentRejected));
            }
        }

        /// <summary>
        /// 设置订单成功 每个商品确认预定成功都会调用该方法
        /// </summary>
        public void MarkAsSuccess(Guid goodsId)
        {
            if (_specificationConfirmStatus == null)
            {
                _specificationConfirmStatus = new List<Guid>();
            }
            if (!_specificationConfirmStatus.Contains(goodsId))
            {
                _specificationConfirmStatus.Add(goodsId);
            }
            //所有商品已经确认完毕
            if(_specificationConfirmStatus.Count==_total.Lines.Length)
            { 
                if (_status != OrderStatus.PaymentSuccess)
                {
                    throw new InvalidOperationException("不正确的订单状态:" + _status);
                }
                ApplyEvent(new OrderSuccessedEvent(_userId,_total, _expressAddressInfo));
            }
        }
        /// <summary>
        /// 设置过期
        /// </summary>
        public void MarkAsExpire()
        {
            if (_status == OrderStatus.ReservationSuccess)
            {
                ApplyEvent(new OrderExpiredEvent(_total));
            }
        }
        /// <summary>
        /// /关闭订单 
        /// </summary>
        public void Close(Guid goodsId)
        {
            if (_specificationConfirmStatus == null)
            {
                _specificationConfirmStatus = new List<Guid>();
            }
            if (!_specificationConfirmStatus.Contains(goodsId))
            {
                _specificationConfirmStatus.Add(goodsId);
            }
            //所有商品已经确认完毕
            if (_specificationConfirmStatus.Count == _total.Lines.Length)
            {
                if (_status != OrderStatus.ReservationSuccess && _status != OrderStatus.PaymentRejected)
                {
                    throw new InvalidOperationException("不正确的订单状态:" + _status);
                }
                ApplyEvent(new OrderClosedEvent());
            }
        }

        #region 私有方法

        
        /// <summary>
        /// 确认订单预定 怎样确定所有的商品预定结果
        /// </summary>
        /// <param name="isReservationSuccess">预定成功与否</param>
        private void ConfirmReservation(bool isReservationSuccess)
        {
            if (_status != OrderStatus.Placed)
            {
                throw new InvalidOperationException("不正确的订单状态:" + _status);
            }
            if (isReservationSuccess)
            {
                ApplyEvent(new OrderReservationConfirmedEvent(_total, OrderStatus.ReservationSuccess));
            }
            else
            {
                ApplyEvent(new OrderReservationConfirmedEvent(_total, OrderStatus.ReservationFailed));
            }
        }
        #endregion

        #region Handle

        private void Handle(OrderPlacedEvent evnt)
        {
            _id = evnt.AggregateRootId;
            _total = evnt.OrderTotal;
            _status = OrderStatus.Placed;
            _userId = evnt.UserId;
            _registrantId = evnt.UserId;
            _expressAddressInfo = evnt.ExpressAddressInfo;
            _reservationExpirationDate = evnt.ReservationExpirationDate;
        }
        private void Handle(OrderReservationConfirmedEvent evnt)
        {
            _status = evnt.OrderStatus;
        }
        private void Handle(OrderPaymentConfirmedEvent evnt)
        {
            _status = evnt.OrderStatus;
        }
        private void Handle(OrderSuccessedEvent evnt)
        {
            _status = OrderStatus.Success;
        }
        private void Handle(OrderExpiredEvent evnt)
        {
            _status = OrderStatus.Expired;
        }
        private void Handle(OrderClosedEvent evnt)
        {
            _status = OrderStatus.Closed;
        }
        #endregion
    }
}
