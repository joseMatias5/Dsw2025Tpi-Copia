using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Validations;

public class ProductValidations
{
    public static void ValidateProduct(ProductModel.RequestProduct request)
    {
        if (string.IsNullOrWhiteSpace(request.sku))
            throw new ArgumentException("SKU cannot be null or empty", nameof(request.sku));
        if (string.IsNullOrWhiteSpace(request.internalCode))
            throw new ArgumentException("Internal code cannot be null or empty", nameof(request.internalCode));
        if (string.IsNullOrWhiteSpace(request.name))
            throw new ArgumentException("Name cannot be null or empty", nameof(request.name));
        if (request.currentUnitPrice <= 0)
            throw new ArgumentException("Current unit price must be greater than zero", nameof(request.currentUnitPrice));
        if (request.stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(request.stockQuantity));
    }
    public async static Task ValidateAddedProduct(ProductModel.RequestProduct request, IRepository _repository)
    {
        if (await _repository.First<Product>(p => p.Sku == request.sku) != null)
            throw new DuplicatedEntityException($"A product with this SKU already exists {request.sku}");
        if (await _repository.First<Product>(p => p.InternalCode == request.internalCode) != null)
            throw new DuplicatedEntityException($"A product with this Internal Code already exists {request.internalCode}");
    }
    public async static Task ValidateExistingProduct(Guid id, IRepository _repository)
    {
        if (await _repository.First<Product>(p => p.Id == id) == null)
            throw new EntityNotFoundException($"Product with ID {id} not found");
    }
}


        
