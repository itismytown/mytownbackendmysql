using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;

namespace mytown.Controllers
{
    [Route("api/shoppingcart/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;

        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderRepository orderRepo,
                                 ILogger<OrderController> logger)
        {
            _orderRepo = orderRepo ?? throw new ArgumentNullException(nameof(orderRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromQuery] int shopperRegId, [FromQuery] string shippingType)
        {
            var result = await _orderRepo.CreateOrderAsync(shopperRegId, shippingType);

            if (result.OrderId == 0)
            {
                return BadRequest("No items in cart to place an order.");
            }

            return Ok(new
            {
                Message = "Order placed successfully",
                OrderId = result.OrderId,
                TrackingId = result.TrackingId
            });
        }

    }
}
