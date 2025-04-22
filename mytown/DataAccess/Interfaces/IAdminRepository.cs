using mytown.Models;

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
    }
}

