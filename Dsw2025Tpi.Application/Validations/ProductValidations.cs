using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Dsw2025Tpi.Application.Validations;

public class ProductValidations
{
    public static void ValidateProduct(ProductModel.RequestProduct request)
    {
        GeneralValidations.ValidateNotNull(request, nameof(request));
        
        GeneralValidations.ValidateNotNull(request.Sku, nameof(request.Sku));
        if (!Regex.IsMatch(request.Sku, @"^SKU-\d{4}$"))
        {
            throw new Exceptions.ArgumentException("Invalid Sku input, valid format = 'SKU-XXXX'");
        }

        GeneralValidations.ValidateNotNull(request.InternalCode, nameof(request.InternalCode));
        if (!Regex.IsMatch(request.InternalCode, @"^INT-\d{4}$"))
        {
            throw new Exceptions.ArgumentException("Invalid InternalCode input, valid format = 'INT-XXXX'");
        }

        GeneralValidations.ValidateText(request.Name, nameof(request.Name));
        GeneralValidations.ValidatePositiveDecimalNumber(request.CurrentUnitPrice.ToString(), nameof(request.CurrentUnitPrice));
        if (request.CurrentUnitPrice <= 0)
            throw new Exceptions.ArgumentException("Current unit price cannot be negative or 0");
        GeneralValidations.ValidatePositiveWholeNumberAndCero(request.StockQuantity.ToString(), nameof(request.StockQuantity));
        GeneralValidations.ValidateOptionalText(request.Description!, nameof(request.Description));
    }
    public async static Task ValidateAddedProduct(ProductModel.RequestProduct request, IRepository _repository)
    {
        ValidateProduct(request);
        if (await _repository.First<Product>(p => p.Sku == request.Sku) != null)
            throw new Exceptions.DuplicatedEntityException($"A product with this SKU already exists {request.Sku}");
        if (await _repository.First<Product>(p => p.InternalCode == request.InternalCode) != null)
            throw new Exceptions.DuplicatedEntityException($"A product with this Internal Code already exists {request.InternalCode}");
    }

    public async static Task ValidateUpdatedProduct(Product product, IRepository _repository)
    {
        ValidateActiveProduct(product);
        if (await _repository.First<Product>(p => p.Sku == product.Sku && p.Id != product.Id) != null)
            throw new Exceptions.DuplicatedEntityException($"A different product with this SKU already exists {product.Sku}");
        if (await _repository.First<Product>(p => p.InternalCode == product.InternalCode && p.Id != product.Id) != null)
            throw new Exceptions.DuplicatedEntityException($"A different product with this Internal Code already exists {product.InternalCode}");
    }

    public async static Task ValidateExistingProduct(Guid id, IRepository _repository)
    {
        GeneralValidations.ValidateGuid(id.ToString(), nameof(id));
        if (await _repository.First<Product>(p => p.Id == id) == null)
            throw new Exceptions.EntityNotFoundException($"Product with ID {id} not found");
    }

    public static void ValidateActiveProduct(Product product)
    {
        GeneralValidations.ValidateNotNull(product, nameof(product));
        if (!product.IsActive)
            throw new Exceptions.InvalidStatusException($"Product with ID {product.Id} is not active");
    }
}


        
