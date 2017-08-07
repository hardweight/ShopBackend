using Buy.ReadModel.Dtos;
using System;

namespace Buy.ReadModel
{
    public interface IOrderQueryService
    {
        Order FindOrder(Guid orderId);
    }
}