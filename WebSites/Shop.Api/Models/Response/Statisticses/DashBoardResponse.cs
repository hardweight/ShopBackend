using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shop.Api.Models.Response.Statisticses
{
    public class DashBoardResponse:BaseApiResponse
    {
        public int UserCount { get; set; }
        public int NewRegCount { get; set; }

        public int AmbassadorCount { get; set; }
        public int NewAmbassadorCount { get; set; }

        public decimal CashTotal { get; set; }
        public decimal LockedCashTotal { get; set; }
        public decimal BenevolenceTotal { get; set; }
        public decimal TodayBenevolenceAddedTotal { get; set; }
    }
}