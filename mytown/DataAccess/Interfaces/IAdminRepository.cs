using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IAdminRepository
    {
        // Admin panel methods

        Task<(IEnumerable<BusinessRegister> records, int totalRecords)> GetBusinessRegistersPaginatedAsync(int page, int pageSize);

        Task<(IEnumerable<ShopperRegister> records, int totalRecords)> GetShopperRegistersPaginatedAsync(int page, int pageSize);

        Task<(int uniqueCities, int uniqueStates, int uniqueCountries)> GetUniqueCountsAsync();

        Task<int> GetBusinessRegisterCountAsync();
        Task<int> GetShoppersRegisterCountAsync();
        Task<int> GetCourierserviceCountAsync();
        Task<(IEnumerable<CourierService> records, int totalRecords)> GetCourierRegistersPaginatedAsync(int page, int pageSize);

        //landing page
        Task<List<LocationStoresDto>> GetLocationsWithCompletedStoresAsync();
    }
}

