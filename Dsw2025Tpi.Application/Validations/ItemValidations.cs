using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Validations;

public class ItemValidations
{
    public static void ValidateItem(OrderItemModel.RequestItem item)
    {
        GeneralValidations.ValidateNotNull(item, nameof(item));
        GeneralValidations.ValidateGuidAndCodes(item.ProductId.ToString(), nameof(item.ProductId));
        GeneralValidations.ValidateWholeNumber(item.Quantity.ToString(), nameof(item.Quantity));

        if (item.Quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(item.Quantity), "Quantity must be greater than zero");
    }

    public static void StockControl(int quantity, Product product)
    {
        if (quantity > product.StockQuantity)
        {
            throw new Exceptions.ApplicationException("Not enough stock");
        }
    }
}
