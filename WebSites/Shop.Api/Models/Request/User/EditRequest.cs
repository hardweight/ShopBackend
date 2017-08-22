using System;

namespace Shop.Api.Models.Request.User
{
    public class EditRequest
    {
        public Guid Id { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
    }
}