using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Interfaces
{
    public interface IProductsManagementService
    {
        Task<ProductModel.ResponseProduct> AddProduct(ProductModel.RequestProduct request);
        Task<ProductModel.ResponseProduct?> DeactivateProduct(Guid id);
        Task<ProductModel.ResponseProduct?> GetProductById(Guid id);
        Task<IEnumerable<ProductModel.ResponseProduct>?> GetProducts();
        Task<ProductModel.ResponseProduct> UpdateProduct(Guid id, ProductModel.RequestProduct request);
    }
}