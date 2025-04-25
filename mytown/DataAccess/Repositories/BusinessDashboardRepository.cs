using mytown.DataAccess.Interfaces;
using mytown.Models.mytown.DataAccess;
using Microsoft.EntityFrameworkCore;
using mytown.Models.DTO_s;


public class BusinessDashboardRepository : IBusinessDashboardRepository
{
    private readonly AppDbContext _context;

    public BusinessDashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    // Get all orders for a specific business owner (store)
    public async Task<List<mytown.Models.Order>> GetAllOrdersForStoreAsync(int storeId)
    {
        // Fetch orders associated with the StoreId, including related OrderDetails, Payments, ShippingDetails, Product, and Store
        var orders = await _context.Orders
            .Where(o => o.OrderDetails.Any(od => od.StoreId == storeId))  // Filtering by StoreId
            .Include(o => o.OrderDetails)  // Include OrderDetails for each order
            .ThenInclude(od => od.Product)  // Include Product for each OrderDetail
            .Include(o => o.OrderDetails)  // Include OrderDetails
            .ThenInclude(od => od.Store)  // Include Store for each OrderDetail
            .Include(o => o.Payments)  // Include Payment details
            .Include(o => o.ShippingDetails)  // Include ShippingDetails for each order
            .ToListAsync();

        return orders;
    }
    public async Task<SalesReportDTO> GetSalesReportByStoreId(int storeId)
    {
        var report = await _context.OrderDetails
            .Where(od => od.StoreId == storeId)
            .GroupBy(od => od.StoreId)
            .Select(g => new SalesReportDTO
            {
                TotalSales = g.Sum(od => od.Quantity * od.Price),
                TotalProductsSold = g.Sum(od => od.Quantity),
                UniqueOrdersCount = g.Select(od => od.OrderId).Distinct().Count()
            })
            .FirstOrDefaultAsync();

        return report;
    }
}


