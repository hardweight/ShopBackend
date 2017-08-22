using ENode.Domain;
using Shop.Common.Enums;
using Shop.Domain.Events.Stores.StoreOrders;
using Shop.Domain.Models.Stores.StoreOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using Xia.Common.Extensions;

namespace Shop.Domain.Models.Stores
{
    /// <summary>
    /// 商家订单 聚合跟
    /// </summary>
    public class StoreOrder:AggregateRoot<Guid>
    {
        private Guid _userId;//下单人
        private Guid _walletId;//付款的钱包信息，用户付款完成后要更新该值，方便以后退款
        private Guid _storeOwnerWalletId;//店主钱包ID
        private StoreOrderInfo _info;//订单信息
        private ExpressAddressInfo _expressAddressInfo;//收货地址
        private RefoundApplyInfo _refoundApplyInfo;//申请退款信息

        private IList<OrderGoodsInfo> _orderGoodses;//订单商品
        private StoreOrderStatus _status;//订单状态

        public StoreOrder(
            Guid id,
            Guid walletId,
            Guid storeOwnerWalletId,
            StoreOrderInfo info,
            ExpressAddressInfo expressAddressInfo,
            IList<OrderGoodsInfo> orderGoodses):base(id)
        {
            ApplyEvent(new StoreOrderCreatedEvent(
                walletId,
                storeOwnerWalletId,
                info,
                expressAddressInfo,
                orderGoodses));
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="expressInfo"></param>
        public void Deliver(ExpressInfo expressInfo)
        {
            if(_status!=StoreOrderStatus.Placed)
            {
                throw new Exception("不正确的包裹状态");
            }
            ApplyEvent(new StoreOrderExpressedEvent(expressInfo));
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        public void ConfirmExpress()
        {
            if(_status!=StoreOrderStatus.Expressing)
            {
                throw new Exception("不正确的包裹状态");
            }
            ApplyEvent(new StoreOrderConfirmExpressedEvent(_storeOwnerWalletId,_orderGoodses.Sum(x=>x.StoreTotal)));
        }

        /// <summary>
        /// 申请仅退款
        /// </summary>
        /// <param name="refoundApplyInfo"></param>
        public void ApplyRefund(RefoundApplyInfo refoundApplyInfo)
        {
            refoundApplyInfo.CheckNotNull(nameof(refoundApplyInfo));

            ApplyEvent(new ApplyRefundedEvent(refoundApplyInfo));
        }

        /// <summary>
        /// 申请退货退款
        /// </summary>
        /// <param name="refoundApplyInfo"></param>
        public void ApplyReturnAndRefund(RefoundApplyInfo refoundApplyInfo)
        {
            refoundApplyInfo.CheckNotNull(nameof(refoundApplyInfo));

            ApplyEvent(new ApplyReturnAndRefundedEvent(refoundApplyInfo));
        }

        /// <summary>
        /// 同意退款
        /// </summary>
        public void AgreeRefund()
        {
            if(_refoundApplyInfo==null)
            {
                throw new Exception("没有退款信息");
            }
            ApplyEvent(new AgreeRefundedEvent(_walletId,_refoundApplyInfo.RefundAmount));
        }
        /// <summary>
        /// 同意退货，请买家返回包裹
        /// </summary>
        public void AgreeReturn()
        {
            if(_refoundApplyInfo==null)
            {
                throw new Exception("没有退货信息");
            }
            ApplyEvent(new AgreeReturnEvent());
        }

        /// <summary>
        /// 获取订单商品总额
        /// </summary>
        /// <returns></returns>
        public decimal GetTotal()
        {
            return _orderGoodses.Sum(x=>x.Total);
        }

        #region Handle
        private void Handle(StoreOrderCreatedEvent evnt)
        {
            _walletId = evnt.WalletId;
            _storeOwnerWalletId = evnt.StoreOwnerWalletId;
            _info = evnt.Info;
            _expressAddressInfo = evnt.ExpressAddressInfo;
            _orderGoodses = evnt.OrderGoodses;
            _status = StoreOrderStatus.Placed;
        }
        private void Handle(StoreOrderExpressedEvent evnt)
        {
            _status = StoreOrderStatus.Expressing;
        }
        private void Handle(StoreOrderConfirmExpressedEvent evnt)
        {
            _status = StoreOrderStatus.Success;
        }
        private void Handle(ApplyRefundedEvent evnt)
        {
            _refoundApplyInfo = evnt.RefoundApplyInfo;
            _status = StoreOrderStatus.OnlyRefund;
        }
        private void Handle(ApplyReturnAndRefundedEvent evnt)
        {
            _refoundApplyInfo = evnt.RefoundApplyInfo;
            _status = StoreOrderStatus.ReturnAndRefund;
        }

        private void Handle(AgreeRefundedEvent evnt)
        {
            _status = StoreOrderStatus.Closed;
        }
        private void Handle(AgreeReturnEvent evnt)
        {
            _status = StoreOrderStatus.AgreeReturn;
        }
        #endregion
    }
}
