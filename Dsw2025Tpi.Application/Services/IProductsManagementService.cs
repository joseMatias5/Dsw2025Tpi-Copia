using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Services
{
    public interface IProductsManagementService
    {
        Task<ProductModel.ResponseProduct> AddProduct(ProductModel.RequestProduct request);
        Task<ProductModel.ResponseProduct?> DeactivateProduct(Guid id);
        Task DeleteProduct<T>(T entity) where T : EntityBase;
        Task<IEnumerable<Product>?> GetProduct();
        Task<Product?> GetProductById(Guid id);
        Task<ProductModel.ResponseProduct> UpdateProduct(Guid id, ProductModel.RequestProduct request);
        Task<T> UpdateProduct<T>(T entity) where T : EntityBase;
    }
}