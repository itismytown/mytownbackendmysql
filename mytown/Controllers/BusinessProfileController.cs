using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using MyTown.Controllers;

namespace mytown.Controllers
{
    [Route("api/business/profile")]
    [ApiController]
    public class BusinessProfileController :ControllerBase
    {
        private readonly IBusinessProfileRepository _businessprofileRepo;
       
        private readonly ILogger<BusinessProfileController> _logger;

        public BusinessProfileController(IBusinessProfileRepository businessprofileRepo,
                                 ILogger<BusinessProfileController> logger)
        {
            _businessprofileRepo = businessprofileRepo ?? throw new ArgumentNullException(nameof(businessprofileRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



       
            [HttpPost("addBusinessProfile")]
        public async Task<IActionResult> AddBusinessProfile([FromBody] businessprofile businessProfile)
        {
            if (businessProfile == null)
            {
                return BadRequest(new { message = "Invalid business profile data" });
            }

            try
            {
                // Add the business profile and get the saved object
                var savedBusinessProfile = await _businessprofileRepo.AddBusinessProfileAsync(businessProfile);

                // Return the saved business profile along with a success message
                return Ok(new
                {
                    message = "Business profile added successfully",
                    data = savedBusinessProfile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the business profile", error = ex.Message });
            }
        }

        [HttpPut("update-banner/{busRegId}")]
        public async Task<IActionResult> UpdateBannerPath(int busRegId, [FromBody] UpdateBannerRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.BannerPath))
                return BadRequest("Banner path cannot be empty");

            bool isUpdated = await _businessprofileRepo.UpdateBannerPathAsync(busRegId, request.BannerPath);

            if (!isUpdated)
                return NotFound($"Business profile with BusRegId {busRegId} not found.");

            return Ok(new { message = "Banner path updated successfully" });
        }

        [HttpPut("update-logo/{busRegId}")]
        public async Task<IActionResult> UpdateLogoPath(int busRegId, [FromBody] UpdateLogoRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.LogoPath))
                return BadRequest("Logo path cannot be empty");

            bool isUpdated = await _businessprofileRepo.UpdateLogoPathAsync(busRegId, request.LogoPath);

            if (!isUpdated)
                return NotFound($"Business profile with BusRegId {busRegId} not found.");

            return Ok(new { message = "Logo path updated successfully" });
        }

        //get sub categories of each business
        [HttpGet("GetProductSubCategories/{BusRegId}")]
        public IActionResult GetProductSubCategories(int busRegId)
        {
            var subCategories = _businessprofileRepo.GetProductSubCategoriesByBusRegId(busRegId);

            if (subCategories == null || !subCategories.Any())
            {
                return NotFound(new { message = "No subcategories found for the given BusRegId." });
            }

            return Ok(subCategories);
        }

        //get profile details using busregid
        [HttpGet("getBusinessProfilesByBusRegId")]
        public async Task<IActionResult> GetBusinessProfilesByBusRegId(int busRegId)
        {
            try
            {
                var businessProfiles = await _businessprofileRepo.GetBusinessProfilesByBusRegIdAsync(busRegId);

                if (businessProfiles == null || !businessProfiles.Any())
                {
                    return NotFound(new { message = "No business profiles found for the given BusRegId" });
                }

                return Ok(new
                {
                    message = "Business profiles retrieved successfully",
                    data = businessProfiles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving business profiles", error = ex.Message });
            }
        }

        //get products for selected category and busregid on preview page
        [HttpGet("by-busreg-and-subcat")]
        public IActionResult GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId)
        {
            var products = _businessprofileRepo.GetProductsByBusRegIdAndSubcatId(busRegId, prodSubcatId);
            if (products == null || !products.Any())
            {
                return NotFound("No products found for the given criteria.");
            }

            return Ok(products);
        }
    }
}
