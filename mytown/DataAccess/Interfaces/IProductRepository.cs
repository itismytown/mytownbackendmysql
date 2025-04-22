using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task<products> CreateProductAsync(products product);
        //    Task<products> GetProductByIdAsync(int productId);
        Task<products> UpdateProductAsync(products product);

        Task DeleteProductAsync(int productId);
        bool UpdateProduct(products product);
        Task<products> GetProductById(int productId); 
        Task<IEnumerable<products>> GetAllProductsAsync(int BusRegId);
    }
}
