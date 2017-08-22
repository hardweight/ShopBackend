using Shop.ReadModel.Payments.Dtos;
using System;

namespace Shop.ReadModel.Payments
{
    public interface IPaymentQueryService
    {
        Payment FindPayment(Guid paymentId);
    }
}