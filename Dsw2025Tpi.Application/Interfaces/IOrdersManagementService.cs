using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface IOrdersManagementService
    {
        Task<OrderModel.ResponseOrder> AddOrder(OrderModel.RequestOrder request);
        Task<OrderModel.ResponseOrder?> ChangeOrderStatus(Guid id, OrderModel.RequestOrder request);
        Task<OrderModel.ResponseOrder> DeleteOrder(Guid id);
        Task<OrderModel.ResponseOrder?> GetOrderById(Guid id);
        Task<IEnumerable<OrderModel.ResponseOrder>?> GetOrders();
        Task<OrderModel.ResponseOrder> UpdateOrder(Guid id, OrderModel.RequestOrder request);
    }
}