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
        
        GeneralValidations.ValidateNotNull(request.sku, nameof(request.sku));
        if (!Regex.IsMatch(request.sku, @"^SKU-\d{4}$"))
        {
            throw new ArgumentException("Invalid sku input, valid format = 'SKU-XXXX'");
        }

        GeneralValidations.ValidateNotNull(request.internalCode, nameof(request.internalCode));
        if (!Regex.IsMatch(request.internalCode, @"^INT-\d{4}$"))
        {
            throw new ArgumentException("Invalid internalCode input, valid format = 'INT-XXXX'");
        }

        GeneralValidations.ValidateText(request.name, nameof(request.name));
        GeneralValidations.ValidatePositiveDecimalNumber(request.currentUnitPrice.ToString(), nameof(request.currentUnitPrice));
        if (request.currentUnitPrice <= 0)
            throw new ArgumentException("current unit price cannot be negative or 0");
        GeneralValidations.ValidateWholeNumber(request.stockQuantity.ToString(), nameof(request.stockQuantity));
        GeneralValidations.ValidateOptionalText(request.description, nameof(request.description));
    }
    public async static Task ValidateAddedProduct(ProductModel.RequestProduct request, IRepository _repository)
    {
        ValidateProduct(request);
        if (await _repository.First<Product>(p => p.Sku == request.sku) != null)
            throw new DuplicatedEntityException($"A product with this SKU already exists {request.sku}");
        if (await _repository.First<Product>(p => p.InternalCode == request.internalCode) != null)
            throw new DuplicatedEntityException($"A product with this Internal Code already exists {request.internalCode}");
    }

    public async static Task ValidateUpdatedProduct(Product product, IRepository _repository)
    {
        ValidateActiveProduct(product);
        if (await _repository.First<Product>(p => p.Sku == product.Sku && p.Id != product.Id) != null)
            throw new DuplicatedEntityException($"A different product with this SKU already exists {product.Sku}");
        if (await _repository.First<Product>(p => p.InternalCode == product.InternalCode && p.Id != product.Id) != null)
            throw new DuplicatedEntityException($"A different product with this Internal Code already exists {product.InternalCode}");
    }

    public async static Task ValidateExistingProduct(Guid id, IRepository _repository)
    {
        GeneralValidations.ValidateGuidAndCodes(id.ToString(), nameof(id));
        if (await _repository.First<Product>(p => p.Id == id) == null)
            throw new EntityNotFoundException($"Product with ID {id} not found");
    }

    public static void ValidateActiveProduct(Product product)
    {
        GeneralValidations.ValidateNotNull(product, nameof(product));
        if (!product.IsActive)
            throw new EntityNotFoundException($"Product with ID {product.Id} is not active");
    }
}


        
