using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Services
{
    public interface IOrdersManagementService
    {
        Task<OrderModel.Response> AddOrder(OrderModel.Request request);
        Task<IEnumerable<Order>?> GetOrder();
        Task<Order?> GetOrderById(Guid id);
    }
}