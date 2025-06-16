using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
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

        public async Task<int> CreateOrderAsync(int shopperRegId, string shippingType, int branchid, decimal cost)
        {
            // Calculate total amount from cart
            var totalAmount = await _context.addtocart
                .Where(c => c.ShopperRegId == shopperRegId && c.orderstatus == "Cart")
                .SumAsync(c => c.product_price * c.prod_qty);

            if (totalAmount == 0)
            {
                return 0; // No items in cart
            }

            // Create new order
            var newOrder = new Order
            {
                ShopperRegId = shopperRegId,
                TotalAmount = totalAmount,
                ShippingType = shippingType,
                OrderStatus = "Pending",
                OrderDate = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            // Move cart items to OrderDetails table
            var cartItems = await _context.addtocart
                .Where(c => c.ShopperRegId == shopperRegId && c.orderstatus == "Cart")
                .ToListAsync();

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

            // Add OrderDetails to the context
            _context.OrderDetails.AddRange(orderDetailsList);
            await _context.SaveChangesAsync(); // Save so that the OrderDetailId is populated

           // var trackingId = Guid.NewGuid().ToString();

            var shippingList = new List<ShippingDetails>();

            foreach (var orderDetail in orderDetailsList)
            {
                var shipping = new ShippingDetails
                {
                    OrderDetailId = orderDetail.OrderDetailId,
                    BranchId = branchid,
                    Shipping_type = shippingType,
                    EstimatedDays = 5,
                    Cost = cost,
                    TrackingId = "", // remove this, this should be enerated by courier person
                    ShippingStatus = "Not Shipped", //put as ready to be shipped
                    OrderId = newOrder.OrderId
                };

                shippingList.Add(shipping);
            }

            // Add all shipping records at once
            _context.ShippingDetails.AddRange(shippingList);
            await _context.SaveChangesAsync();

            foreach (var shippingDetail in shippingList)
            {
                await SendEmailToCourier(shippingDetail.BranchId, shippingDetail.ShippingDetailId);
            }

            return (newOrder.OrderId);
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
