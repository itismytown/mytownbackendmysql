﻿using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
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

        [HttpGet("orders/{storeId}")]
        public async Task<ActionResult<List<BusinessDashboardDto>>> GetStoreOrdersReport(int storeId)
        {
            var result = await _dashboardRepository.GetStoreOrdersReport(storeId);
            if (result == null || result.Count == 0)
                return NotFound("No orders found for this store.");

            return Ok(result);
        }

        // GET api/businessdashboard/locationcounts/{storeId}
        [HttpGet("locationcounts/{storeId}")]
        public async Task<ActionResult<LocationStatsDto>> GetLocationCountsByStoreId(int storeId)
        {
            var result = await _dashboardRepository.GetLocationCountsByStoreIdAsync(storeId);
            if (result == null)
                return NotFound("No shoppers found for this store.");

            return Ok(result);
        }

        [HttpGet("salesreport/{storeId}")]
        public async Task<IActionResult> GetSalesReport(int storeId)
        {
            var salesReport = await _dashboardRepository.GetSalesReportByStoreId(storeId);
            if (salesReport == null)
            {
                return NotFound();
            }

            return Ok(salesReport);
        }
    }
}
