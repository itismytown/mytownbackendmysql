using mytown.Models;
using mytown.Models.DTO_s;
using System.Threading.Tasks;

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
        Task<IEnumerable<ProductDto>> GetDiscountedProductsAsync();

        Task<IEnumerable<ProductDto>> GetProductsBySubCategoryAsync(int subCategoryId);

        Task SaveProductViewAsync(int shopperId, int productId);
    }
}
