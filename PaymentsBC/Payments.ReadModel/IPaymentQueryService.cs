using Payments.ReadModel.Dtos;
using System;

namespace Payments.ReadModel
{
    public interface IPaymentQueryService
    {
        Payment FindPayment(Guid paymentId);
    }
}