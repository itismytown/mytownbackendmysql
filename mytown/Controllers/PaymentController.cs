using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using Stripe;

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
        private string GetCurrencyFromCountry(string countryName)
        {
            // Example: Map country name to currency code
            var countryCurrencyMapping = new Dictionary<string, string>
    {
        { "United States", "usd" },  // Country -> Currency code
        { "India", "inr" },
        { "United Kingdom", "gbp" },
        { "European Union", "eur" },
        { "Japan", "jpy" },
        // Add other countries and currencies as needed
    };

            // Return the currency code if found, otherwise return null
            if (countryCurrencyMapping.ContainsKey(countryName))
            {
                return countryCurrencyMapping[countryName];
            }

            return null;  // Return null if no valid currency is found
        }
        [HttpPost("create-payment-intent")]
        public ActionResult CreatePaymentIntent([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                // Get the currency code based on the country name
                string currency = GetCurrencyFromCountry(paymentRequest.CountryName);

                // If no valid currency is found, default to USD
                if (currency == null)
                {
                    currency = "usd";
                }

                // Create the PaymentIntent options
                var options = new PaymentIntentCreateOptions
                {
                    Amount = paymentRequest.Amount,  // Amount in cents (e.g., $10 = 1000)
                    Currency = currency,             // Currency based on country name
                    PaymentMethodTypes = new List<string> { "card" },
                };

                // Create the payment intent using Stripe's service
                var service = new PaymentIntentService();
                PaymentIntent intent = service.Create(options);

                // Return the client secret to the frontend for confirming payment
                return Ok(new { clientSecret = intent.ClientSecret });
            }
            catch (StripeException e)
            {
                // Return error if Stripe API call fails
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
