﻿using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;
using mytown.Services;

namespace mytown.DataAccess.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;


        public PaymentRepository(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;

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
                PaymentStatus = "Completed", 
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

        public List<BusinessRegisterDto> GetStoreDetailsByOrderId(int orderId)
        {
            var storeDetails = _context.OrderDetails
                                       .Where(od => od.OrderId == orderId)
                                       .Select(od => new BusinessRegisterDto
                                       {
                                           BusRegId = od.Store.BusRegId,
                                           Businessname = od.Store.Businessname,
                                           BusinessUsername = od.Store.BusinessUsername,
                                           BusEmail = od.Store.BusEmail,
                                           BusMobileNo = od.Store.BusMobileNo
                                       })
                                       .Distinct()
                                       .ToList();

            return storeDetails;
        }

        public List<ShippingDetails> GetShippingDetailsByOrderId(int orderId)
        {
            return _context.ShippingDetails
                           .Where(s => s.OrderId == orderId)
                           .ToList();
        }

        public async Task SendEmailToCourier(int branchId, int shippingDetailId)
        {
            var courierInfo = await _context.CourierBranches
                .Where(cb => cb.BranchId == branchId)
                .Select(cb => new
                {
                    cb.CourierName,
                    cb.CourierId,
                    CourierEmail = cb.CourierService.CourierEmail
                })
                .FirstOrDefaultAsync();

            if (courierInfo != null && !string.IsNullOrEmpty(courierInfo.CourierEmail))
            {
                await _emailService.SendEmailToCourierAsync(
                    courierInfo.CourierEmail,
                    courierInfo.CourierName,
                    shippingDetailId
                );
            }
        }

    }
}
