using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;


namespace Dsw2025Tpi.Application.Services;

public class ProductsManagementService : IProductsManagementService
{
    // This service will handle the business logic for managing products.
    // It will interact with the repository to perform CRUD operations on products.
    // It will also handle validation and any other business rules related to products.
    // Example method to add a product
    IRepository _repository;
    public ProductsManagementService(IRepository repository)
    {
        _repository = repository;
    }
    public async Task<ProductModel.Response?> GetProductById(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        return product != null ?
            new ProductModel.Response(product.Id, product.Sku, product.InternalCode, product.Name, product.Description,
                product.CurrentUnitPrice, product.StockQuantity) :
            null;

    }

    public async Task<IEnumerable<ProductModel.Response>?> GetProducts()
    {

        var activeProducts = await _repository.GetFiltered<Product>(p => p.IsActive);

        /*if (!activeProducts.Any())
            throw new Exceptions.ApplicationException("There are no active products");*/

        /*return activeProducts.Select(p => new ProductModel.Response(
            p.Id,
            p.Sku,
            p.InternalCode,
            p.Name,
            p.Description,
            p.CurrentUnitPrice,
            p.StockQuantity));*/
        return (await _repository
            .GetFiltered<Product>(p => p.IsActive))?
            .Select(p => new ProductModel.Response(
                p.Id,
                p.Sku,
                p.InternalCode,
                p.Name,
                p.Description,
                p.CurrentUnitPrice,
                p.StockQuantity));
    }



    public async Task<ProductModel.Response> AddProduct(ProductModel.Request request)
    {
        if (string.IsNullOrWhiteSpace(request.sku) ||
            string.IsNullOrWhiteSpace(request.internalCode) ||
            string.IsNullOrWhiteSpace(request.name) ||
            request.currentUnitPrice <= 0 ||
            request.stockQuantity <= 0
            )
        {
            throw new ArgumentException("A product cannot be created with those values");
        }

        if (await _repository.First<Product>(p => p.Sku == request.sku) != null)
            throw new DuplicatedEntityException($"A product with this SKU already exists {request.sku}");
        if (await _repository.First<Product>(p => p.InternalCode == request.internalCode) != null)
            throw new DuplicatedEntityException($"A product with this Internal Code already exists {request.internalCode}");

        var product = new Product(request.sku, request.internalCode, request.name, request.description, request.currentUnitPrice, request.stockQuantity);
        await _repository.Add(product);
        return new ProductModel.Response(
            product.Id,
            product.Sku,
            product.InternalCode,
            product.Name,
            product.Description,
            product.CurrentUnitPrice,
            product.StockQuantity);
    }

    public async Task<ProductModel.Response> UpdateProduct(Guid id, ProductModel.Request request)
    {
        var product = await _repository.First<Product>(p => p.Id == id);
        if (product == null)
            throw new EntityNotFoundException($"Product with ID {id} not found");

        if (string.IsNullOrWhiteSpace(request.sku) ||
            string.IsNullOrWhiteSpace(request.internalCode) ||
            string.IsNullOrWhiteSpace(request.name) ||
            request.currentUnitPrice <= 0 ||
            request.stockQuantity <= 0
            )
        {
            throw new ArgumentException("A product cannot be updated with those values");
        }

        product.Sku = request.sku;
        product.InternalCode = request.internalCode;
        product.Name = request.name;
        product.Description = request.description;
        product.CurrentUnitPrice = request.currentUnitPrice;
        product.StockQuantity = request.stockQuantity;

        var updated = await _repository.Update(product);
        return new ProductModel.Response(
            updated.Id,
            updated.Sku,
            updated.InternalCode,
            updated.Name,
            updated.Description,
            updated.CurrentUnitPrice,
            updated.StockQuantity
        );
    }

    public async Task<ProductModel.Response> DeleteProduct(Guid id)
    {
        var product = await _repository.First<Product>(p => p.Id == id);
        if (product == null)
            throw new EntityNotFoundException($"Product with ID {id} not found");
        product.IsActive = false;
        var deleted = await _repository.Update(product);
        return new ProductModel.Response(
            deleted.Id,
            deleted.Sku,
            deleted.InternalCode,
            deleted.Name,
            deleted.Description,
            deleted.CurrentUnitPrice,
            deleted.StockQuantity
        );
    }
    //Para el PATCH
    public async Task<ProductModel.Response?> DeactivateProduct(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        if (product == null)
            return null;

        product.IsActive = false;
        var updated = await _repository.Update(product);

        return new ProductModel.Response(
            updated.Id,
            updated.Sku,
            updated.InternalCode,
            updated.Name,
            updated.Description,
            updated.CurrentUnitPrice,
            updated.StockQuantity
        );
    }
}
