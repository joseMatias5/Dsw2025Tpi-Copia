using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Application.Dtos;


namespace Dsw2025Tpi.Application.Services
{
    public class Services
    {
        private readonly EfRepository _repository;

        public Services(EfRepository repository)
        {
            _repository = repository;
        }

        #region Product
        public async Task<Product?> GetProductById(Guid id)
        {
            return await _repository.GetById<Product>(id);
        }

        public async Task<List<Product>?> GetProduct()
        {
            return await _repository.GetAll<Product>();
        }

        public async Task<ProductModel.Response> AddProduct(ProductModel.Request request)
        {
            if (string.IsNullOrWhiteSpace(request.) ||
                string.IsNullOrWhiteSpace(request.Title) ||
                string.IsNullOrWhiteSpace(request.Author))
            {
                throw new ArgumentException("Valores para el producto no válidos");
            }

            var exist = await _repository.First<Product>(p => p.Sku == request.Sku);
            if (exist != null) throw new DuplicatedEntityException($"Ya existe un producto con el Sku {request.Sku}");

            var product = new Product(request.Title, request.Author, request.Year, request.ISBN);
            await _repository.AddAsync(Product);
            return new ProductModel.Response(Product.Id);
        }
        #endregion
        #region order
        public async Task<Order?> GetOrderById(Guid id)
        {
            return await _repository.GetById<Order>(id);
        }

        public async Task<List<Order>?> GetOrder()
        {
            return await _repository.GetAll<Order>();

        }

        public async Task<OrderModel.Response> AddOrder(OrderModel.Request request)
        {
            var order = new Order(request.Date, request.);
            await _repository.AddAsync(Order);
            return new OrderItemModel.Response(Order.);
        }
        #endregion

        #region OrderItem
        public async Task<OrderItem?> GetOrderItemById(Guid id)
        {
            return await _repository.GetById<OrderItem>(id);
        }

        public async Task<List<OrderItem>?> GetOrderItem()
        {
            return await _repository.GetAll<OrderItem>();
        }

        public async Task<OrderItemModel.Response> AddOrderItem(OrderItemModel.Request request)
        {
            var orderItem = new OrderItem(request.Name, request.Email);
            await _repository.AddAsync(OrderItem);
            return new OrderItemModel.Response(OrderItem.ReferenceEquals|);
        }
        #endregion

        #region Customer
        public async Task<Customer?> GetCustomerById(Guid id)
        {
            return await _repository.GetById<Customer>(id);
        }

        public async Task<List<Customer>?> GetCustomer()
        {
            return await _repository.GetAll<Customer>();
        }

        public async Task<CustomerModel.Response> AddCustomer(CustomerModel.Request request)
        {
            var customer = new Customer(request.Name, request.Email);
            await _repository.AddAsync(Customer);
            return new Customer.Response(Customer.ReferenceEquals |);
        }
        #endregion

    }
}
