using Shop.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Api.Models.Request.Wallet
{
    public class BenevolenceTransfersRequest
    {
        public BenevolenceTransferType Type { get; set; }
        public int Page { get; set; }
    }
}