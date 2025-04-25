using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mytown.Controllers
{
    [ApiController]
    [Route("api/businessdashboard")]
    public class BusinessDashboardController : ControllerBase
    {
        private readonly IBusinessDashboardRepository _dashboardRepository;

        public BusinessDashboardController(IBusinessDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        // GET: api/BusinessDashboard/orders/{storeId}
        [HttpGet("orderhistory/{storeId}")]
        public async Task<ActionResult<List<Order>>> GetOrdersForStore(int storeId)
        {
            var orders = await _dashboardRepository.GetAllOrdersForStoreAsync(storeId);

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the given store.");
            }

            return Ok(orders);
        }

        //[HttpGet("salesreport/{storeId}")]
        //public async Task<IActionResult> GetSalesReport(int storeId)
        //{
        //    var salesReport = await GetSalesReportByStoreId(storeId);
        //    if (salesReport == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(salesReport);
        //}
    }
}
