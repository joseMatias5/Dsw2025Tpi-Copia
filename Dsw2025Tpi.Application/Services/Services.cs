using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;


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

        public async Task<IEnumerable<Product>?> GetProduct()
        {
            return await _repository.GetAll<Product>();
        }
        
        public async Task<ProductModel.Response> AddProduct(ProductModel.Request request)
        {
            if (string.IsNullOrWhiteSpace(request.Sku) ||
                string.IsNullOrWhiteSpace(request.InternalCode) ||
                string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.CurrentUnitPrice.ToString()) ||
                string.IsNullOrWhiteSpace(request.StockQuantity.ToString()))
            {
                throw new ArgumentException("Invalid values for product");
            }

            var exist = await _repository.First<Product>(p => p.Sku == request.Sku);
            if (exist != null) throw new DuplicatedEntityException($"A product with that Sku already exists {request.Sku}");
            if (exist != null) throw new DuplicatedEntityException($"A product with that Internal Code already exists {request.InternalCode}");

            var product = new Product(request.Sku, request.InternalCode, request.Name, request?.Description, request.CurrentUnitPrice,request.StockQuantity);
            await _repository.Add(product);
            return new ProductModel.Response(product.Id,product.Sku,product.InternalCode,product.Name,product?.Description,product.CurrentUnitPrice,product.StockQuantity);
        }
        #endregion
        
        #region Order
        public async Task<Order?> GetOrderById(Guid id)
        {
            return await _repository.GetById<Order>(id);
        }

        public async Task<IEnumerable<Order>?> GetOrder()
        {
            return await _repository.GetAll<Order>();

        }

        public async Task<OrderModel.Response> AddOrder(OrderModel.Request request)
        {
            //falta lo de totalAmount (que se calcula)
            if (string.IsNullOrWhiteSpace(request.Date.ToString()) ||
                string.IsNullOrWhiteSpace(request.ShippingAddress) ||
                string.IsNullOrWhiteSpace(request.BillingAddress))
            {
                throw new ArgumentException("Invalid values for order");
            }

            var order = new Order(request.Date, request.ShippingAddress, request.BillingAddress,request?.Notes, request.TotalAmount);
            await _repository.Add(order);
            return new OrderModel.Response(order.Id,order.Date,order.ShippingAddress,order.BillingAddress,order?.Notes,order.TotalAmount);
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
