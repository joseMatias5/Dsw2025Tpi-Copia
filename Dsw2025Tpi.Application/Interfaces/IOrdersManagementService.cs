using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface IOrdersManagementService
    {
        Task<OrderModel.Response> AddOrder(OrderModel.Request request);
        Task<OrderModel.Response?> ChangeOrderStatus(Guid id, OrderModel.Request request);
        Task<OrderModel.Response> DeleteOrder(Guid id);
        Task<OrderModel.Response?> GetOrderById(Guid id);
        Task<IEnumerable<OrderModel.Response>?> GetOrders();
        Task<OrderModel.Response> UpdateOrder(Guid id, OrderModel.Request request);
    }
}