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
using Dsw2025Tpi.Application.Services;

namespace Dsw2025Tpi.Application.Interfaces;

public class ProductsManagementServices : IProductsManagementService
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

    public async Task<ProductModel.ResponseProduct> AddProduct(ProductModel.RequestProduct request)
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
        if (await _repository.First<Product>(p => p.Sku == request.Sku) != null)
            throw new DuplicatedEntityException($"Ya existe un producto con el SKU {request.Sku}");

        if (await _repository.First<Product>(p => p.InternalCode == request.InternalCode) != null)
            throw new DuplicatedEntityException($"Ya existe un producto con el código interno {request.InternalCode}");

        var product = new Product(request.Sku, request.InternalCode, request.Name, request.Description, request.CurrentUnitPrice, request.StockQuantity);
        await _repository.Add(product);
        return new ProductModel.ResponseProduct(product.Id, product.Sku, product.InternalCode, product.Name, product.Description, product.CurrentUnitPrice, product.StockQuantity);
    }
    public async Task<T> UpdateProduct<T>(T entity) where T : EntityBase
         => await _repository.Update(entity);

    public async Task DeleteProduct<T>(T entity) where T : EntityBase
        => await _repository.Delete(entity);

    public async Task<ProductModel.ResponseProduct> UpdateProduct(Guid id, ProductModel.RequestProduct request)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            throw new System.ApplicationException("Producto no encontrado.");
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("SKU y nombre son obligatorios.");
        product.Sku = request.Sku;
        product.InternalCode = request.InternalCode;
        product.Name = request.Name;
        product.CurrentUnitPrice = request.CurrentUnitPrice;
        product.StockQuantity = request.StockQuantity;

        var updated = await _repository.Update(product);
        return new ProductModel.ResponseProduct(
            updated.Id,
            updated.Sku,
            updated.InternalCode,
            updated.Name,
            updated.Description,
            updated.CurrentUnitPrice,
            updated.StockQuantity
        );
    }
    public async Task<ProductModel.ResponseProduct?> DeactivateProduct(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            return null;

        product.IsActive = false;
        var updated = await _repository.Update(product);

        return new ProductModel.ResponseProduct(
            updated.Id, updated.Sku, updated.InternalCode, updated.Name, updated.Description, updated.CurrentUnitPrice, updated.StockQuantity
        );
    }

}