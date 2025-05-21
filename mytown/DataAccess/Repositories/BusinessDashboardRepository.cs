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

  
    // get sales history
    public async Task<List<BusinessDashboardDto>> GetStoreOrdersReport(int storeId)
    {
        var query = from od in _context.OrderDetails
                    join o in _context.Orders on od.OrderId equals o.OrderId
                    join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
                    join p in _context.products on od.ProductId equals p.product_id
                    join pay in _context.Payments on o.OrderId equals pay.OrderId into payJoin
                    from payment in payJoin.DefaultIfEmpty() // LEFT JOIN
                    join sd in _context.ShippingDetails on od.OrderDetailId equals sd.OrderDetailId into sdJoin
                    from shipping in sdJoin.DefaultIfEmpty() // LEFT JOIN
                    where od.StoreId == storeId
                    select new BusinessDashboardDto
                    {
                        OrderId = o.OrderId,
                        OrderDetailId = od.OrderDetailId,
                        OrderDate = o.OrderDate,
                        CustomerName = s.Username,
                        ProductName = p.product_name,
                        Quantity = od.Quantity,
                        Amount = od.Quantity * od.Price,
                        PaymentStatus = payment != null ? payment.PaymentStatus : "Unpaid",
                        Address = s.Address,
                        Town = s.Town,
                        City = s.City,
                        State = s.State,
                        Country = s.Country,
                        DeliveryType = shipping != null ? shipping.Shipping_type : "Not Shipped",
                        DeliveryStatus = o.OrderStatus
                    };

        return await query.ToListAsync();
    }
    
    // get order, sales, product, customer count
    public async Task<SalesReportDTO> GetSalesReportByStoreId(int storeId)
    {
        var reportData = await (from od in _context.OrderDetails
                                join o in _context.Orders on od.OrderId equals o.OrderId
                                where od.StoreId == storeId
                                select new
                                {
                                    od.Quantity,
                                    od.Price,
                                    od.OrderId,
                                    o.ShopperRegId
                                }).ToListAsync();

        if (!reportData.Any())
        {
            return new SalesReportDTO
            {
                TotalSales = 0,
                TotalProductsSold = 0,
                UniqueOrdersCount = 0,
                UniqueShoppersCount = 0
            };
        }

        return new SalesReportDTO
        {
            TotalSales = reportData.Sum(x => x.Quantity * x.Price),
            TotalProductsSold = reportData.Sum(x => x.Quantity),
            UniqueOrdersCount = reportData.Select(x => x.OrderId).Distinct().Count(),
            UniqueShoppersCount = reportData.Select(x => x.ShopperRegId).Distinct().Count()
        };
    }

    // to get location counts - tiwns, cities, states, country
    public async Task<LocationStatsDto> GetLocationCountsByStoreIdAsync(int storeId)
    {
        var query = from od in _context.OrderDetails
                    where od.StoreId == storeId
                    join o in _context.Orders on od.OrderId equals o.OrderId
                    join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
                    select s;

        var uniqueShoppers = await query.Distinct().ToListAsync();

        var result = new LocationStatsDto
        {
            TownCount = uniqueShoppers.Select(x => x.Town).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count(),
            CityCount = uniqueShoppers.Select(x => x.City).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count(),
            StateCount = uniqueShoppers.Select(x => x.State).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count(),
            CountryCount = uniqueShoppers.Select(x => x.Country).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count()
        };

        return result;
    }

}


