using Shop.Api.ViewModels.Carts;
using Dtos = Shop.ReadModel.Carts.Dtos;

namespace Shop.Api.Extensions
{
    public static class CartViewModelCartExtension
    {
        public static CartViewModel ToCartModel(this Dtos.Cart value)
        {
            return new CartViewModel()
            {
                Id = value.Id,
                UserId=value.UserId
            };
        }
    }
}