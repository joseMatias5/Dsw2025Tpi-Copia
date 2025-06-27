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
    public class ProductsManagementServices
    {
        private readonly IRepository _repository;

        public ProductsManagementServices(IRepository repository)
        {
            _repository = repository;
        }
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

            var product = new Product(request.Sku, request.InternalCode, request.Name, request.Description, request.CurrentUnitPrice,request.StockQuantity);
            await _repository.Add(product);
            return new ProductModel.Response(product.Id,product.Sku,product.InternalCode,product.Name,product.Description,product.CurrentUnitPrice,product.StockQuantity);
        }
        public async Task<T> UpdateProduct<T>(T entity) where T : EntityBase
             => await _repository.Update(entity);

        public async Task DeleteProduct<T>(T entity) where T : EntityBase
            => await _repository.Delete(entity);

    }
}
