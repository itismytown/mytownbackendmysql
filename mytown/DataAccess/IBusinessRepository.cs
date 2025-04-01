using Microsoft.AspNetCore.Mvc;
using mytown.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.DataAccess
{
    public interface IBusinessRepository
    {
        Task<BusinessRegister> AddBusinessRegisterAsync(BusinessRegister newBusiness);
        Task<BusinessRegister> GetBusinessByEmailAsync(string email);
        Task<ActionResult<IEnumerable<businesscategoriescs>>> GetBusinessCategories();
        Task<ActionResult<IEnumerable<businessservices>>> GetBusinessServices();
        Task<IEnumerable<product_sub_categories>> GetSubCategoriesByBuscatIdAsync(int buscatId);
        Task AddOrUpdateSubcategoryImageAsync(subcategoryimages_busregid subcategoryImage);
        Task<List<subcategoryimages_busregid>> GetSubcategoryImagesByBusRegIdAsync(int busRegId);
        Task<products> CreateProductAsync(products product);
        Task<products> GetProductById(int productId);
        Task<IEnumerable<products>> GetAllProductsAsync(int busRegId);
        Task DeleteProductAsync(int productId);
        Task<businessprofile> AddBusinessProfileAsync(businessprofile businessProfile);
        Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath);
        List<product_sub_categories> GetProductSubCategoriesByBusRegId(int busRegId);
        bool UpdateProduct(products updatedProduct);
        Task<List<busprofilepreview>> GetBusinessProfilesByBusRegIdAsync(int busRegId);
        IEnumerable<products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId);
    }
}
