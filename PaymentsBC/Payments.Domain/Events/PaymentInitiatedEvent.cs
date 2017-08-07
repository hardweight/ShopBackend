﻿using System;
using System.Collections.Generic;
using ENode.Eventing;
using Payments.Domain.Modes;

namespace Payments.Events
{
    /// <summary>
    /// 支付初始化完成 事件
    /// </summary>
    [Serializable]
    public class PaymentInitiatedEvent : DomainEvent<Guid>
    {
        public Guid OrderId { get; private set; }
        public string Description { get; private set; }
        public decimal TotalAmount { get; private set; }
        public IEnumerable<PaymentItem> Items { get; private set; }

        public PaymentInitiatedEvent() { }
        public PaymentInitiatedEvent(Guid orderId, string description, decimal totalAmount, IEnumerable<PaymentItem> items)
        {
            OrderId = orderId;
            Description = description;
            TotalAmount = totalAmount;
            Items = items;
        }
    }
}
