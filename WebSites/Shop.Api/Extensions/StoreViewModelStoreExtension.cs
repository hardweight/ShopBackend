using Shop.Api.ViewModels.Store;
using Dtos = Shop.ReadModel.Stores.Dtos;

namespace Shop.Api.Extensions
{
    public static class StoreViewModelStoreExtension
    {
        public static StoreViewModel ToStoreModel(this Dtos.Store value)
        {
            return new StoreViewModel()
            {
                Id=value.Id,
                UserId=value.UserId,
                AccessCode=value.AccessCode,
                Name=value.Name,
                Description=value.Description,
                Region=value.Region,
                Address=value.Address
            };
        }

        
    }
}