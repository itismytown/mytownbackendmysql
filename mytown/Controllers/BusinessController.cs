using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess;
using mytown.Models;
using mytown.Services.Validation;
using mytown.Services;
using System.Text.Json;


namespace mytown.Controllers
{
    [Route("api/business")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BusinessController> _logger;
        private readonly IBusinessRegistrationValidator _registrationValidator;
        private readonly IVerificationLinkBuilderbusiness _verificationLinkBuilderbusiness;

        public BusinessController(IBusinessRepository businessRepository, 
            IEmailService emailService,
          IConfiguration configuration,
          ILogger<BusinessController> logger,
          IBusinessRegistrationValidator registrationValidator,
          IVerificationLinkBuilderbusiness verificationLinkBuilderbusiness)
        {
            _businessRepository = businessRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _registrationValidator = registrationValidator;
            _verificationLinkBuilderbusiness = verificationLinkBuilderbusiness;
        }

        [HttpPost("businessregister")]
        public async Task<IActionResult> RegisterBusiness([FromBody] BusinessRegisterDto businessRegisterDto)
        {
            List<string> validationErrors = _registrationValidator.Validate(businessRegisterDto);
            if (validationErrors.Count > 0)
            {
                _logger.LogWarning("Validation failed for {Email}: {Errors}", businessRegisterDto.BusEmail, validationErrors);
                return BadRequest(new { errors = validationErrors });
            }

            try
            {
                if (await _businessRepository.IsEmailTaken(businessRegisterDto.BusEmail))
                {
                    return Conflict(new { error = "This email is already registered. Try logging in instead." });
                }

                //string token = Guid.NewGuid().ToString();
                //DateTime expiry = DateTime.UtcNow.AddHours(24);
                //string frontendBaseUrl = _configuration["FrontendBaseUrl"];
                //string verificationLink = _verificationLinkBuilderbusiness.BuildLink(frontendBaseUrl, token);

                //string jsonPayload = JsonSerializer.Serialize(businessRegisterDto);

                //var pending = new PendingBusinessVerification
                //{
                //    Email = businessRegisterDto.BusEmail,
                //    Token = token,
                //    ExpiryDate = expiry,
                //    JsonPayload = jsonPayload
                //};

                //await _businessRepository.SavePendingVerification(pending);
                //await _emailService.SendVerificationEmail(businessRegisterDto.BusEmail, verificationLink);

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(businessRegisterDto.Password);

                var newBusiness = new BusinessRegister
                {
                    BusinessUsername = businessRegisterDto.BusinessUsername,
                    Businessname = businessRegisterDto.Businessname,
                    LicenseType = businessRegisterDto.LicenseType,
                    Gstin = businessRegisterDto.Gstin,
                    BusservId = businessRegisterDto.BusservId,
                    BuscatId = businessRegisterDto.BuscatId,
                    Town = businessRegisterDto.Town,
                    BusMobileNo = businessRegisterDto.BusMobileNo,
                    BusEmail = businessRegisterDto.BusEmail,
                    Address1 = businessRegisterDto.Address1,
                    Address2 = businessRegisterDto.Address2,
                    businessCity = businessRegisterDto.businessCity,
                    businessState = businessRegisterDto.businessState,
                    businessCountry = businessRegisterDto.businessCountry,
                    Password = hashedPassword,
                    IsEmailVerified = true
                };

                await _businessRepository.RegisterBusiness(newBusiness);

                _logger.LogInformation("Verification email sent to {Email}", businessRegisterDto.BusEmail);
                return Ok(new { message = "Verification email sent! Please check your inbox." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", businessRegisterDto.BusEmail);
                return StatusCode(500, new { error = "Something went wrong. Please try again later." });
            }
        }

        [HttpGet("verify-business-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var pending = await _businessRepository.FindPendingVerificationByToken(token);
                if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
                {
                    return BadRequest(new { error = "Invalid or expired verification link." });
                }

                var businessDto = JsonSerializer.Deserialize<BusinessRegisterDto>(pending.JsonPayload);
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(businessDto.Password);

                var newBusiness = new BusinessRegister
                {
                    BusinessUsername = businessDto.BusinessUsername,
                    Businessname = businessDto.Businessname,
                    LicenseType = businessDto.LicenseType,
                    Gstin = businessDto.Gstin,
                    BusservId = businessDto.BusservId,
                    BuscatId = businessDto.BuscatId,
                    Town = businessDto.Town,
                    BusMobileNo = businessDto.BusMobileNo,
                    BusEmail = businessDto.BusEmail,
                    Address1 = businessDto.Address1,
                    Address2 = businessDto.Address2,
                    businessCity = businessDto.businessCity,
                    businessState = businessDto.businessState,
                    businessCountry = businessDto.businessCountry,
                    Password = hashedPassword,
                    IsEmailVerified = true
                };

                await _businessRepository.RegisterBusiness(newBusiness);
                await _businessRepository.DeletePendingVerification(token);

                _logger.LogInformation("Email verified and business registered for {Email}", newBusiness.BusEmail);
                return Ok(new { message = "Your email is verified and your business account is created!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying token: {Token}", token);
                return StatusCode(500, new { error = "Could not verify email. Please try again later." });
            }
        }


        //[HttpPost("resend-business-verification")]
        //public async Task<IActionResult> ResendVerificationEmail([FromQuery] string token)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(token))
        //        {
        //            _logger.LogWarning("Resend verification requested without a token.");
        //            return BadRequest(new { error = "Verification token is missing." });
        //        }

        //        var existingVerification = await _businessRepository.FindVerificationByToken(token);
        //        if (existingVerification == null)
        //        {
        //            _logger.LogWarning("Resend verification requested: token {Token} not found.", token);
        //            return BadRequest(new { error = "Verification token not found. Please register again." });
        //        }

        //        string email = existingVerification.Email;
        //        _logger.LogInformation("Resend verification requested for {Email} (old token: {Token})", email, token);

        //        // Remove old verification record and generate a new one
        //        await _businessRepository.RemoveVerification(existingVerification);
        //        var newVerification = await _businessRepository.GenerateEmailVerification(email);

        //        string frontendBaseUrl = _configuration["FrontendBaseUrl"];
        //        if (string.IsNullOrWhiteSpace(frontendBaseUrl))
        //        {
        //            _logger.LogError("Frontend base URL is not configured properly.");
        //            return StatusCode(500, new { message = "Frontend base URL is missing. Please contact support." });
        //        }

        //        // Build and send new verification link
        //        string verificationLink = _verificationLinkBuilderbusiness.BuildLink(frontendBaseUrl, newVerification.VerificationToken);
        //        _logger.LogInformation("New verification link generated for {Email}: {VerificationLink}", email, verificationLink);

        //        await _emailService.SendVerificationEmail(email, verificationLink);
        //        return Ok(new { message = $"A new verification email has been sent to {email}." });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Exception during resend verification for token: {Token}", token);
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}

        //get business owner home page with busregid
        [HttpGet("businessregister/{busRegId}")]
        public async Task<IActionResult> GetBusinessById(int busRegId)
        {
            try
            {
                var business = await _businessRepository.GetBusinessByIdAsync(busRegId);

                if (business == null)
                {
                    return NotFound(new { error = "Business not found with the given BusRegId." });
                }

                return Ok(business);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving business with BusRegId {BusRegId}", busRegId);
                return StatusCode(500, new { error = "An error occurred while retrieving the business details." });
            }
        }


        [HttpGet("BusinessCategories")]
        public async Task<ActionResult<IEnumerable<businesscategoriescs>>> GetBusinessCategories()
        {
            var categories = await _businessRepository.GetBusinessCategories();
            return Ok(categories);
        }



        [HttpPost("Add_Products")]
        public async Task<IActionResult> CreateProduct([FromBody] products product)
        {
            if (product == null)
            {
                return BadRequest("Product data is required.");
            }

            await _businessRepository.CreateProductAsync(product);
            return Ok(new { productId = product.product_id });
        }

        [HttpDelete("deleteProduct")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            try
            {
                // Use the repository to delete the product
                await _businessRepository.DeleteProductAsync(productId);

                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting product: {ex.Message}");

                // Return a generic error response
                return StatusCode(500, new { message = "An error occurred while deleting the product." });
            }
        }

        [HttpPut("updateProduct")]
        public IActionResult UpdateProduct([FromBody] products updatedProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid product data" });
            }

            var isUpdated = _businessRepository.UpdateProduct(updatedProduct);
            if (!isUpdated)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(new { message = "Product updated successfully" });
        }
    }

}
