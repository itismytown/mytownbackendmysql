using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessDashboardRepository
    {
       // Task<List<mytown.Models.Order>> GetAllOrdersForStoreAsync(int storeId);
        Task<SalesReportDTO> GetSalesReportByStoreId(int storeId);
        Task<List<BusinessDashboardDto>> GetStoreOrdersReport(int storeId);
        Task<LocationStatsDto> GetLocationCountsByStoreIdAsync(int storeId);

        Task<List<ProductDto>> GetProductsWithPurchasedCountAsync(
         int busRegId,
         string searchText = null,
         string sortBy = "id",
         string sortDirection = "asc",
         int page = 1,
         int pageSize = 10);

        Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(int storeId);

    }
}
