using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod)
        {
            // Check if the order exists
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            // Create new payment entry
            var payment = new Payments
            {
                OrderId = orderId,
                AmountPaid = amountPaid,
                PaymentMethod = paymentMethod,
                PaymentStatus = "Completed", // Assuming successful payment
                PaymentDate = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            _context.SaveChanges();

            // Update order status to "Paid"
            order.OrderStatus = "Paid";
            _context.Orders.Update(order);
            _context.SaveChanges();



            return payment;
        }

    }
}
