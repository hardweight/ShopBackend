using System;
using System.Collections.Generic;
using ENode.Domain;
using Payments.Events;

namespace Payments.Domain.Modes
{
    /// <summary>
    /// 支付聚合跟
    /// </summary>
    [Serializable]
    public class Payment:AggregateRoot<Guid>
    {
        private Guid _orderId;
        private PaymentState _state;
        private string _description;
        private decimal _totalAmount;
        private IEnumerable<PaymentItem> _items;

        public Payment(Guid id, Guid orderId, string description, decimal totalAmount, IEnumerable<PaymentItem> items) : base(id)
        {
            ApplyEvent(new PaymentInitiatedEvent(orderId, description, totalAmount, items));
        }

        public void Complete()
        {
            if (_state != PaymentState.Initiated)
            {
                throw new InvalidOperationException();
            }
            ApplyEvent(new PaymentCompletedEvent(this, _orderId));
        }
        public void Cancel()
        {
            if (_state != PaymentState.Initiated)
            {
                throw new InvalidOperationException();
            }
            ApplyEvent(new PaymentRejectedEvent(_orderId));
        }

        private void Handle(PaymentInitiatedEvent evnt)
        {
            _id = evnt.AggregateRootId;
            _orderId = evnt.OrderId;
            _description = evnt.Description;
            _totalAmount = evnt.TotalAmount;
            _state = PaymentState.Initiated;
            _items = evnt.Items;
        }
        private void Handle(PaymentCompletedEvent evnt)
        {
            _state = PaymentState.Completed;
        }
        private void Handle(PaymentRejectedEvent evnt)
        {
            _state = PaymentState.Rejected;
        }
    }
}
