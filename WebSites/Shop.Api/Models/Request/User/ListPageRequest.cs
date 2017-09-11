using Shop.Common.Enums;

namespace Shop.Api.Models.Request.User
{
    public class ListPageRequest
    {
        public UserRole Role { get; set; }
        public int Page { get; set; }
        public string Mobile { get; set; }
    }
}