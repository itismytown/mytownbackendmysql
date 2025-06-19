using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services;
using Stripe;

namespace mytown.DataAccess.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public OrderRepository(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<int> CreateOrderAsync(int shopperRegId, List<StoreShippingSelection> shippingSelections)
        {
            var cartItems = await _context.addtocart
                .Where(c => c.ShopperRegId == shopperRegId && c.orderstatus == "Cart")
                .ToListAsync();

            if (!cartItems.Any())
                return 0;

            decimal totalAmount = cartItems.Sum(c => c.product_price * c.prod_qty);

            var newOrder = new Order
            {
                ShopperRegId = shopperRegId,
                TotalAmount = totalAmount,
                ShippingType = "Multiple", // or leave blank or null
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            List<orderdetails> orderDetailsList = new List<orderdetails>();

            foreach (var item in cartItems)
            {
                var orderDetail = new orderdetails
                {
                    OrderId = newOrder.OrderId,
                    ProductId = item.product_id,
                    StoreId = item.BusRegId,
                    Quantity = item.prod_qty,
                    Price = item.product_price
                };
                orderDetailsList.Add(orderDetail);
            }

            _context.OrderDetails.AddRange(orderDetailsList);
            await _context.SaveChangesAsync();

            List<ShippingDetails> shippingList = new List<ShippingDetails>();

            foreach (var orderDetail in orderDetailsList)
            {
                var shippingSelection = shippingSelections
                    .FirstOrDefault(s => s.StoreId == orderDetail.StoreId);

                if (shippingSelection == null)
                    throw new Exception($"No shipping selected for store {orderDetail.StoreId}");

                var shipping = new ShippingDetails
                {
                    OrderId = newOrder.OrderId,
                    OrderDetailId = orderDetail.OrderDetailId,
                    BranchId = shippingSelection.BranchId,
                    Shipping_type = shippingSelection.ShippingType,
                    EstimatedDays = 5, // can be dynamic later
                    Cost = shippingSelection.Cost,
                    TrackingId = "",
                    ShippingStatus = "Ready to Ship"
                };

                shippingList.Add(shipping);
            }

            _context.ShippingDetails.AddRange(shippingList);
            await _context.SaveChangesAsync();

            foreach (var shippingDetail in shippingList)
            {
                await SendEmailToCourier(shippingDetail.BranchId, shippingDetail.ShippingDetailId);
            }

            return newOrder.OrderId;
        }


        private async Task SendEmailToCourier(int branchId, int shippingDetailId)
        {
            var courierPerson = await _context.CourierBranches
                .Where(cb => cb.BranchId == branchId)
                .Select(cb => new { cb.BranchEmailId, cb.CourierName })
                .FirstOrDefaultAsync();

            if (courierPerson != null && !string.IsNullOrEmpty(courierPerson.BranchEmailId))
            {
               

                await _emailService.SendEmailToCourierAsync(courierPerson.BranchEmailId,courierPerson.CourierName,shippingDetailId);
            }
        }

    }
}
