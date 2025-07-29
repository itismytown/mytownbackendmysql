using mytown.Models;
using static mytown.Models.busprofilepreview;

namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessProfileRepository
    {
        Task<List<busprofilepreview>> GetBusinessProfilesByBusRegIdAsync(int busRegId);

        IEnumerable<products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId);

        Task<businessprofile> AddBusinessProfileAsync(businessprofile businessProfile);

        Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath);
        Task<bool> UpdateLogoPathAsync(int busRegId, string logoPath);

        List<product_sub_categories> GetProductSubCategoriesByBusRegId(int busRegId);
    }
}

