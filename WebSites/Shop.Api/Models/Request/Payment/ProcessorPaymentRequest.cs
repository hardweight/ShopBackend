using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Api.Models.Request.Payment
{
    public class ProcessorPaymentRequest
    {
        public Guid PaymentId { get; set; }
    }
}