using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Shop.Common;
using ECommon.Components;
using ECommon.Dapper;
using Payments.ReadModel.Dtos;

namespace Payments.ReadModel
{
    [Component]
    public class PaymentQueryService : IPaymentQueryService
    {
        public Payment FindPayment(Guid paymentId)
        {
            using (var connection = GetConnection())
            {
                return connection.QueryList<Payment>(new { Id = paymentId }, ConfigSettings.PaymentTable).FirstOrDefault();
            }
        }

        private IDbConnection GetConnection()
        {
            return new SqlConnection(ConfigSettings.ConnectionString);
        }
    }
}