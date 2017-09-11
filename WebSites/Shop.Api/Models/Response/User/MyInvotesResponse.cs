using System;
using System.Collections.Generic;

namespace Shop.Api.Models.Response.User
{
    public class MyInvotesResponse:BaseApiResponse
    {
        public IList<dynamic> MyInvotes { get; set; }
    }
}