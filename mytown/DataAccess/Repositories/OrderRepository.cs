using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateOrderAsync(int shopperRegId, string shippingType)
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
            var newOrder = new order
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
                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();

            //// Remove items from cart after placing order
            //_context.addtocart.RemoveRange(cartItems);
            //await _context.SaveChangesAsync();

            return newOrder.OrderId; // Return the created OrderId
        }
    }
}
