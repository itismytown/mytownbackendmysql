using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models;

namespace mytown.Controllers
{
    [Route("api/shoppingcart/payment")]
    [ApiController]
    public class PaymentController: ControllerBase
    {
        private readonly IPaymentRepository _paymentRepo;

        private readonly ILogger<OrderController> _logger;

        public PaymentController(IPaymentRepository paymentRepo,
                                ILogger<OrderController> logger)
        {
            _paymentRepo = paymentRepo ?? throw new ArgumentNullException(nameof(paymentRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpPost("AddPayment")]
        public IActionResult AddPayment([FromBody] PaymentRequestModel model)
        {
            if (model == null || model.OrderId <= 0 || model.AmountPaid <= 0 || string.IsNullOrEmpty(model.PaymentMethod))
            {
                return BadRequest("Invalid payment details.");
            }

            var payment = _paymentRepo.AddPayment(model.OrderId, model.AmountPaid, model.PaymentMethod);

            return Ok(new { message = "Payment successful!", paymentId = payment.PaymentId });
        }

    }
}
