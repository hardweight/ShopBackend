using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Api.Models.Response.Wallet
{
    public class GetBenevolenceTransfersResponse:BaseApiResponse
    {
        public IList<BenevolenceTransfer> BenevolenceTransfers { get; set; }
    }

    public class BenevolenceTransfer
    {
        public string Number { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string Remark { get; set; }
        public string Type { get; set; }
        public string CreatedOn { get; set; }
        public string Direction { get; set; }
    }
}