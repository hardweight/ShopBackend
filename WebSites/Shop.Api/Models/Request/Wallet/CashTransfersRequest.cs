using Shop.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Api.Models.Request.Wallet
{
    public class CashTransfersRequest
    {
        public CashTransferType Type { get; set; }
        public int Page { get; set; }
    }
}