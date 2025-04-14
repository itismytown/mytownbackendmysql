using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task<products> CreateProductAsync(products product);
    //    Task<products> GetProductByIdAsync(int productId);
        Task<bool> UpdateProductAsync(products product);
        Task DeleteProductAsync(int productId);
    }
}
