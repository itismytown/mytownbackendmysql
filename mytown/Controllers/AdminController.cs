using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;

namespace mytown.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IAdminRepository _adminRepo;

        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminRepository adminRepo,
                                 ILogger<AdminController> logger)
        {
            _adminRepo = adminRepo ?? throw new ArgumentNullException(nameof(adminRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("GetBusinessRegistersPaginated")]
        public async Task<IActionResult> GetBusinessRegistersPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 2)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new { message = "Invalid pagination parameters." });
            }

            var (records, totalRecords) = await _adminRepo.GetBusinessRegistersPaginatedAsync(page, pageSize);

            return Ok(new
            {
                data = records,
                totalRecords,
                currentPage = page,
                pageSize
            });
        }



        // Add API to get unique counts for cities, states, and countries
        [HttpGet("GetUniqueCounts")]
        public async Task<IActionResult> GetUniqueCounts()
        {
            try
            {
                // Get unique counts from the repository
                var (uniqueCities, uniqueStates, uniqueCountries) = await _adminRepo.GetUniqueCountsAsync();

                // Return the result
                return Ok(new
                {
                    message = "Unique counts retrieved successfully",
                    data = new
                    {
                        uniqueCities = uniqueCities,
                        uniqueStates = uniqueStates,
                        uniqueCountries = uniqueCountries
                    }
                });
            }
            catch (Exception ex)
            {
                // If there's an error, return a 500 status code
                return StatusCode(500, new { message = "Error retrieving unique counts", error = ex.Message });
            }
        }

        [HttpGet("getBusinessRegisterCount")]
        public async Task<IActionResult> GetBusinessRegisterCount()
        {
            var count = await _adminRepo.GetBusinessRegisterCountAsync();
            return Ok(new { count });
        }

        [HttpGet("getShoppersRegisterCount")]
        public async Task<IActionResult> getShoppersRegisterCount()
        {
            var count = await _adminRepo.GetShoppersRegisterCountAsync();
            return Ok(new { count });
        }



        [HttpGet("getShopperRegistersPaginated")]
        public async Task<IActionResult> GetShopperRegistersPaginated(int page = 1, int pageSize = 10)
        {
            // Validate page and pageSize
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest(new { message = "Page and page size must be greater than 0." });
            }

            // Call the repository method to fetch paginated shopper registers
            var (shopperRegisters, totalRecords) = await _adminRepo.GetShopperRegistersPaginatedAsync(page, pageSize);

            // Check if there are any records
            if (shopperRegisters == null || !shopperRegisters.Any())
            {
                return Ok(new { data = new List<object>(), message = "No shopper registers found.", totalRecords = 0 });
            }

            // Return the paginated list of shoppers as JSON
            return Ok(new
            {
                data = shopperRegisters,
                totalRecords,
                currentPage = page,
                pageSize
            });
        }

    }
}
