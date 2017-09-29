using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Api.Models.Request.Wallet
{
    public class AcceptTransferRequest
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
    }
}