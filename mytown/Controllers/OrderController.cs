using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models.DTO_s;

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
       //[HttpPost("CreateOrder")]
       //public async Task<IActionResult> CreateOrder([FromQuery] int shopperRegId, [FromQuery] string shippingType, [FromQuery] int branchid, [FromQuery] decimal cost)
       // {
       //     int orderId = await _orderRepo.CreateOrderAsync(shopperRegId, shippingType, branchid, cost);

       //     if (orderId == 0)
       //     {
       //         return BadRequest("No items in cart to place an order.");
       //     }

       //     return Ok(new
       //     {
       //         Message = "Order placed successfully",
       //         OrderId = orderId
       //         // TrackingId = result.TrackingId
       //     });
       // }


        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var orderId = await _orderRepo.CreateOrderAsync(request.ShopperRegId, request.ShippingSelections);

            if (orderId == 0)
                return BadRequest("No items in cart.");

            return Ok(new { Message = "Order placed successfully", OrderId = orderId });
        }


    }
}
