using ECommon.Components;
using ECommon.Dapper;
using Shop.Common;
using Shop.ReadModel.Payments.Dtos;
using System;
using System.Linq;

namespace Shop.ReadModel.Payments
{
    [Component]
    public class PaymentQueryService : BaseQueryService, IPaymentQueryService
    {
        public Payment FindPayment(Guid paymentId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Payment>(new { Id = paymentId }, ConfigSettings.PaymentTable).FirstOrDefault();
            }
        }

        
    }
}