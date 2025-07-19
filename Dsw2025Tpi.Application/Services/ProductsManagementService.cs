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
    public async Task<ProductModel.ResponseProduct?> GetProductById(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        return product != null ?
            new ProductModel.ResponseProduct(product.Id, product.Sku, product.InternalCode, product.Name, product.Description,
                product.CurrentUnitPrice, product.StockQuantity) :
            null;

    }

    public async Task<IEnumerable<ProductModel.ResponseProduct>?> GetProducts()
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
            .Select(p => new ProductModel.ResponseProduct(
                p.Id,
                p.Sku,
                p.InternalCode,
                p.Name,
                p.Description,
                p.CurrentUnitPrice,
                p.StockQuantity)
            );
    }

    public async Task<ProductModel.ResponseProduct> AddProduct(ProductModel.RequestProduct request)
    {
        Validations.ProductValidations.ValidateProduct(request);
        await Validations.ProductValidations.ValidateAddedProduct(request, _repository);

        var product = new Product(request.sku, request.internalCode, request.name, request.description, request.currentUnitPrice, request.stockQuantity);
        await _repository.Add(product);
        return new ProductModel.ResponseProduct(
            product.Id,
            product.Sku,
            product.InternalCode,
            product.Name,
            product.Description,
            product.CurrentUnitPrice,
            product.StockQuantity);
    }

    public async Task<ProductModel.ResponseProduct> UpdateProduct(Guid id, ProductModel.RequestProduct request)
    {
        await Validations.ProductValidations.ValidateExistingProduct(id, _repository);
        var product = await _repository.First<Product>(p => p.Id == id);
        Validations.ProductValidations.ValidateProduct(request);

        product.Sku = request.sku;
        product.InternalCode = request.internalCode;
        product.Name = request.name;
        product.Description = request.description;
        product.CurrentUnitPrice = request.currentUnitPrice;
        product.StockQuantity = request.stockQuantity;

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

    public async Task<ProductModel.ResponseProduct> DeleteProduct(Guid id)
    {
        var product = await _repository.First<Product>(p => p.Id == id);
        await Validations.ProductValidations.ValidateExistingProduct(id, _repository);
        product.IsActive = false;
        var deleted = await _repository.Update(product);
        return new ProductModel.ResponseProduct(
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
    public async Task<ProductModel.ResponseProduct?> DeactivateProduct(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        await Validations.ProductValidations.ValidateExistingProduct(id, _repository);
        if (product == null)
            return null;

        product.IsActive = false;
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
}
