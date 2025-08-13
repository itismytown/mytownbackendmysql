﻿using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IAdminRepository
    {
        // Admin panel methods

        Task<(IEnumerable<object> Records, int TotalRecords)> GetBusinessRegistersPaginatedAsync(int page, int pageSize);

        Task<(List<BusinessRegister> Records, int TotalRecords)> GetBusinessesstoresByStatusPaginatedAsync(string status, int page, int pageSize);
        Task<(List<BusinessRegister> Records, int TotalRecords)> GetBusinessesservicesByStatusPaginated(string status, int page, int pageSize);

        Task<Dictionary<string, int>> Businessprofilestatuscounts();

        Task<bool> UpdateProfileStatusbyAdminAsync(int busRegId, string status);
        Task<(IEnumerable<ShopperRegister> records, int totalRecords)> GetShopperRegistersPaginatedAsync(int page, int pageSize);
        Task<bool> UpdateShopperStatusAsync(int shopperId, string newStatus);
        Task<ShopperRegister?> GetShopperByIdAsync(int shopperId);
        Task<(int uniqueTowns, int uniqueCities, int uniqueStates, int uniqueCountries)> GetUniqueCountsAsync();

        Task<int> GetBusinessRegisterCountAsync();
        Task<int> GetShoppersRegisterCountAsync();
        Task<int> GetCourierserviceCountAsync();
        Task<(IEnumerable<CourierService> records, int totalRecords)> GetCourierRegistersPaginatedAsync(int page, int pageSize);

        //landing page
        Task<List<LocationStoresDto>> GetLocationsWithCompletedStoresAsync();
    }
}

