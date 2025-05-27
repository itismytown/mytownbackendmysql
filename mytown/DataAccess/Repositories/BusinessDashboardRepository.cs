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
    //public async Task<List<BusinessDashboardDto>> GetStoreOrdersReport(int storeId)
    //{
    //    var query = from od in _context.OrderDetails
    //                join o in _context.Orders on od.OrderId equals o.OrderId
    //                join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
    //                join p in _context.products on od.ProductId equals p.product_id
    //                join pay in _context.Payments on o.OrderId equals pay.OrderId into payJoin
    //                from payment in payJoin.DefaultIfEmpty() // LEFT JOIN
    //                join sd in _context.ShippingDetails on od.OrderDetailId equals sd.OrderDetailId into sdJoin
    //                from shipping in sdJoin.DefaultIfEmpty() // LEFT JOIN
    //                where od.StoreId == storeId
    //                select new BusinessDashboardDto
    //                {
    //                    OrderId = o.OrderId,
    //                    OrderDetailId = od.OrderDetailId,
    //                    OrderDate = o.OrderDate,
    //                    CustomerName = s.Username,
    //                    ProductName = p.product_name,
    //                    Quantity = od.Quantity,
    //                    Amount = od.Quantity * od.Price,
    //                    PaymentStatus = payment != null ? payment.PaymentStatus : "Unpaid",
    //                    Address = s.Address,
    //                    Town = s.Town,
    //                    City = s.City,
    //                    State = s.State,
    //                    Country = s.Country,
    //                    DeliveryType = shipping != null ? shipping.Shipping_type : "Not Shipped",
    //                    DeliveryStatus = o.OrderStatus
    //                };

    //    return await query.ToListAsync();
    //}



    public async Task<List<BusinessDashboardDto>> GetStoreOrdersReport(int storeId)
    {
        var query = from od in _context.OrderDetails
                    join o in _context.Orders on od.OrderId equals o.OrderId
                    join s in _context.ShopperRegisters on o.ShopperRegId equals s.ShopperRegId
                    join p in _context.products on od.ProductId equals p.product_id
                    join pay in _context.Payments on o.OrderId equals pay.OrderId into payJoin
                    from payment in payJoin.DefaultIfEmpty()
                    join sd in _context.ShippingDetails on od.OrderDetailId equals sd.OrderDetailId into sdJoin
                    from shipping in sdJoin.DefaultIfEmpty()
                    where od.StoreId == storeId && o.OrderStatus == "Paid"
                    select new BusinessDashboardDto
                    {
                        OrderId = o.OrderId,
                        OrderDetailId = od.OrderDetailId,
                        OrderDate = o.OrderDate,
                        CustomerName = s.Username,
                        ShopperId = s.ShopperRegId,
                        ProductId = p.product_id,
                        ProductName = p.product_name,
                        Quantity = od.Quantity,
                        Amount = od.Quantity * od.Price,
                        PaymentStatus = payment != null ? payment.PaymentStatus : "Unpaid",
                        TransactionId = payment.PaymentId,
                        Address = s.Address,
                        Town = s.Town,
                        City = s.City,
                        State = s.State,
                        Country = s.Country,
                        DeliveryType = shipping != null ? shipping.Shipping_type : "Standard",
                        ShippingStatus = shipping != null ? shipping.ShippingStatus : "Not Shipped"
                    };

        var result = await query.ToListAsync();

        // Categorize paid orders based on shipping status and date
        foreach (var order in result)
        {
            order.OrderCategory =
                (order.OrderDate >= DateTime.UtcNow.AddDays(-2) && order.ShippingStatus == "Not Shipped") ? "New" :
                (order.ShippingStatus == "Not Shipped") ? "Pending" :
                (order.ShippingStatus == "In Transit") ? "In Progress" :
                (order.ShippingStatus == "Delivered") ? "Completed" :
                "Other";
        }

        return result;
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


    // show products tab data
    public async Task<List<ProductDto>> GetProductsWithPurchasedCountAsync(int busRegId, string searchText = null, string sortBy = "id", string sortDirection = "asc", int page = 1, int pageSize = 10)
    {
        var query = from p in _context.products
                    where p.BusRegId == busRegId
                    // Left join orderdetails on product id and store id
                    join od in _context.OrderDetails.Where(od => od.StoreId == busRegId)
                        on p.product_id equals od.ProductId into odGroup
                    select new
                    {
                        Product = p,
                        PurchasedCount = odGroup.Sum(x => (int?)x.Quantity) ?? 0
                    };

        // Apply search filter
        if (!string.IsNullOrEmpty(searchText))
        {
            query = query.Where(x =>
                x.Product.product_name.Contains(searchText) ||
                x.Product.product_subject.Contains(searchText) ||
                x.Product.product_id.ToString().Contains(searchText)
            );
        }

        // Sorting
        bool isAsc = sortDirection.ToLower() == "asc";
        query = sortBy?.ToLower() switch
        {
            "price" => isAsc ? query.OrderBy(x => x.Product.product_cost) : query.OrderByDescending(x => x.Product.product_cost),
            "quantity" => isAsc ? query.OrderBy(x => x.Product.product_quantity) : query.OrderByDescending(x => x.Product.product_quantity),
            "purchasedcount" => isAsc ? query.OrderBy(x => x.PurchasedCount) : query.OrderByDescending(x => x.PurchasedCount),
            "name" => isAsc ? query.OrderBy(x => x.Product.product_name) : query.OrderByDescending(x => x.Product.product_name),
            "id" => isAsc ? query.OrderBy(x => x.Product.product_id) : query.OrderByDescending(x => x.Product.product_id),
            _ => query.OrderBy(x => x.Product.product_id)
        };

        // Pagination
        int skip = (page - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

        // Project to DTO
        var result = await query.Select(x => new ProductDto
        {
            ProductId = x.Product.product_id,
            ProductType = x.Product.prod_subcat_id,
            ProductName = x.Product.product_name,
            ProductAmount = x.Product.product_cost,
            Quantity = x.Product.product_quantity,
            PurchasedCount = x.PurchasedCount,
            ProductImage = x.Product.product_image,
            // You can add Rating & Review here if you have that data
        }).ToListAsync();

        return result;
    }

    //Show customer data analtics

    public async Task<CustomerAnalyticsDto> GetCustomerAnalyticsAsync(int storeId)
    {
        // Visited and Purchased
        var purchasedCustomers = await _context.OrderDetails
            .Where(od => od.StoreId == storeId &&
                         (od.Order.Payments.Any() || od.Order.ShippingDetails.Any()))
            .Select(od => od.Order.ShopperRegId)
            .Distinct()
            .ToListAsync();

        // Visited but Not Purchased
        var notPurchasedCustomers = await _context.OrderDetails
            .Where(od => od.StoreId == storeId &&
                         !od.Order.Payments.Any() &&
                         !od.Order.ShippingDetails.Any())
            .Select(od => od.Order.ShopperRegId)
            .Distinct()
            .ToListAsync();

        // Frequent Customers (Purchased more than once)
        var frequentCustomers = await _context.OrderDetails
            .Where(od => od.StoreId == storeId && od.Order.Payments.Any())
            .GroupBy(od => new { od.Order.ShopperRegId, od.Order.ShopperRegister })
            .Where(g => g.Count() > 1)
            .Select(g => new FrequentCustomerDto
            {
                Name = g.Key.ShopperRegister.Username,
                PhoneNumber = g.Key.ShopperRegister.PhoneNumber,
                PurchaseCount = g.Count()
            })
            .ToListAsync();

        // Customers Who Purchased (Names and Phones)
        var customersWhoPurchased = await _context.OrderDetails
            .Where(od => od.StoreId == storeId &&
                         (od.Order.Payments.Any() || od.Order.ShippingDetails.Any()))
            .Select(od => new CustomerDto
            {
                Name = od.Order.ShopperRegister.Username,
                PhoneNumber = od.Order.ShopperRegister.PhoneNumber
            })
            .Distinct()
            .ToListAsync();

        return new CustomerAnalyticsDto
        {
            CustomersVisitedAndPurchased = purchasedCustomers.Count,
            CustomersVisitedButNotPurchased = notPurchasedCustomers.Count,
            FrequentCustomers = frequentCustomers,
            CustomersWhoPurchased = customersWhoPurchased
        };
    }


}


