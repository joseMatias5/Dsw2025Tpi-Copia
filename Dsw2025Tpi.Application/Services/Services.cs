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

            var exist = await _repository.First<Book>(p => p.ISBN == request.ISBN);
            if (exist != null) throw new DuplicatedEntityException($"Ya existe un producto con el ISBN {request.ISBN}");

            var book = new Book(request.Title, request.Author, request.Year, request.ISBN);
            await _repository.AddAsync(book);
            return new ProductModel.Response(book.Id);
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

        public async Task<OrderModel.Response> AddLoan(OrderModel.Request request)
        {
            var order = new Order(request.Date, request.);
            await _repository.AddAsync(loan);
            return new LoanModel.Response(loan.Id);
        }
        #endregion
    }
}
