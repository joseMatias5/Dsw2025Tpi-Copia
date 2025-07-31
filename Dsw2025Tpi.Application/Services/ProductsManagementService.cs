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
using Microsoft.Extensions.Logging;


namespace Dsw2025Tpi.Application.Services;

public class ProductsManagementService : IProductsManagementService
{
    IRepository _repository;
    private readonly ILogger<ProductsManagementService> _logger;
    public ProductsManagementService(IRepository repository,
        ILogger<ProductsManagementService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    public async Task<ProductModel.ResponseProduct?> GetProductById(Guid id)
    {
        _logger.LogInformation("Consulta de producto por Id: {id}", id);
        await ProductValidations.ValidateExistingProduct(id, _repository);
        var product = await _repository.GetById<Product>(id);
        
        return product != null ?
            new ProductModel.ResponseProduct(product.Id, product.Sku, product.InternalCode, product.Name, product.Description,
                product.CurrentUnitPrice, product.StockQuantity, product.IsActive) :
            null;
    }

    public async Task<IEnumerable<ProductModel.ResponseProduct>?> GetProducts()
    {
        _logger.LogInformation("Consulta de productos");
        var activeProducts = await _repository.GetFiltered<Product>(p => p.IsActive);
        Validations.GeneralValidations.ValidateNotNull(activeProducts, nameof(activeProducts));

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
        _logger.LogInformation("Solicitud de agregar productos");
        Validations.GeneralValidations.ValidateNotNull(request, nameof(request));
        await ProductValidations.ValidateAddedProduct(request, _repository);

        var product = new Product(request.Sku, request.InternalCode, request.Name, request.Description, request.CurrentUnitPrice, request.StockQuantity);
        await _repository.Add(product);
        _logger.LogInformation("Creacion de producto exitosa");
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
        _logger.LogInformation("Modificacion de producto con Id: {id}", id);
        Validations.GeneralValidations.ValidateNotNull(request, nameof(request));
        await ProductValidations.ValidateExistingProduct(id, _repository);
        ProductValidations.ValidateProduct(request);
        var product = await _repository.First<Product>(p => p.Id == id);

        product!.Sku = request.Sku;
        product.InternalCode = request.InternalCode;
        product.Name = request.Name;
        product.Description = request.Description;
        product.CurrentUnitPrice = request.CurrentUnitPrice;
        product.StockQuantity = request.StockQuantity;

        await ProductValidations.ValidateUpdatedProduct(product!, _repository);

        var updated = await _repository.Update(product);
        _logger.LogInformation("Modificacion de producto exitosa");
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

    //Para el PATCH
    public async Task<ProductModel.ResponseProduct?> DeactivateProduct(Guid id)
    {
        _logger.LogInformation("Desactivacion de producto con Id: {id}", id);
        var product = await _repository.GetById<Product>(id);
        await ProductValidations.ValidateExistingProduct(id, _repository);
        ProductValidations.ValidateActiveProduct(product!);
        product!.IsActive = false;
        var updated = await _repository.Update(product);
        _logger.LogInformation("Desactivacion de producto exitosa");
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
