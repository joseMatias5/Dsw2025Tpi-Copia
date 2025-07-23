using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Application.Validations;


namespace Dsw2025Tpi.Application.Services;

public class ProductsManagementService : IProductsManagementService
{
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
                product.CurrentUnitPrice, product.StockQuantity, product.IsActive) :
            null;

    }

    public async Task<IEnumerable<ProductModel.ResponseProduct>?> GetProducts()
    {

        var activeProducts = await _repository.GetFiltered<Product>(p => p.IsActive);

        return (await _repository
            .GetFiltered<Product>(p => p.IsActive))?
            .Select(p => new ProductModel.ResponseProduct(
                p.Id,
                p.Sku,
                p.InternalCode,
                p.Name,
                p.Description,
                p.CurrentUnitPrice,
                p.StockQuantity,
                p.IsActive)
            );
    }

    public async Task<ProductModel.ResponseProduct> AddProduct(ProductModel.RequestProduct request)
    {
        ProductValidations.ValidateProduct(request);
        await ProductValidations.ValidateAddedProduct(request, _repository);

        var product = new Product(request.sku, request.internalCode, request.name, request.description, request.currentUnitPrice, request.stockQuantity);
        await _repository.Add(product);
        return new ProductModel.ResponseProduct(
            product.Id,
            product.Sku,
            product.InternalCode,
            product.Name,
            product.Description,
            product.CurrentUnitPrice,
            product.StockQuantity,
            product.IsActive);
    }

    public async Task<ProductModel.ResponseProduct> UpdateProduct(Guid id, ProductModel.RequestProduct request)
    {
        await ProductValidations.ValidateExistingProduct(id, _repository);
        var product = await _repository.First<Product>(p => p.Id == id);
        ProductValidations.ValidateProduct(request);

        product!.Sku = request.sku;
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
            updated.StockQuantity,
            updated.IsActive
        );
    }

    public async Task<ProductModel.ResponseProduct> DeleteProduct(Guid id)
    {
        var product = await _repository.First<Product>(p => p.Id == id);
        await ProductValidations.ValidateExistingProduct(id, _repository);
        product!.IsActive = false;
        var deleted = await _repository.Update(product);
        return new ProductModel.ResponseProduct(
            deleted.Id,
            deleted.Sku,
            deleted.InternalCode,
            deleted.Name,
            deleted.Description,
            deleted.CurrentUnitPrice,
            deleted.StockQuantity,
            deleted.IsActive
        );
    }
    //Para el PATCH
    public async Task<ProductModel.ResponseProduct?> DeactivateProduct(Guid id)
    {
        var product = await _repository.GetById<Product>(id);
        await ProductValidations.ValidateExistingProduct(id, _repository);
        ProductValidations.ValidateActiveProduct(product!);
        product!.IsActive = false;
        var updated = await _repository.Update(product);

        return new ProductModel.ResponseProduct(
            updated.Id,
            updated.Sku,
            updated.InternalCode,
            updated.Name,
            updated.Description,
            updated.CurrentUnitPrice,
            updated.StockQuantity,
            updated.IsActive
        );
    }
}
