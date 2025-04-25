namespace mytown.DataAccess.Interfaces
{
    public interface IBusinessDashboardRepository
    {
        Task<List<mytown.Models.Order>> GetAllOrdersForStoreAsync(int storeId);
        //Task<SalesReportDTO> GetSalesReportByStoreId(int storeId);
    }
}
