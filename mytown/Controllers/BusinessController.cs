using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess;
using mytown.Models;
using mytown.Services.Validation;
using mytown.Services;

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
        private readonly IVerificationLinkBuilder _verificationLinkBuilder;

        public BusinessController(IBusinessRepository businessRepository, 
            IEmailService emailService,
          IConfiguration configuration,
          ILogger<BusinessController> logger,
          IBusinessRegistrationValidator registrationValidator,
          IVerificationLinkBuilder verificationLinkBuilder)
        {
            _businessRepository = businessRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _registrationValidator = registrationValidator;
            _verificationLinkBuilder = verificationLinkBuilder;
        }
      
        [HttpPost("businessregister")]
        public async Task<IActionResult> RegisterBusiness([FromBody] BusinessRegisterDto businessRegisterDto)
        {
            // Validate the registration model
            List<string> validationErrors = _registrationValidator.Validate(businessRegisterDto);
            if (validationErrors.Count > 0)
            {
                _logger.LogWarning("Registration validation errors for {Email}: {Errors}", businessRegisterDto.BusEmail, validationErrors);
                return BadRequest(new { errors = validationErrors });
            }

            try
            {
                // Hash the password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(businessRegisterDto.Password);

                // Create new business registration instance
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
                    NewPassword = hashedPassword,
                    IsEmailVerified = false
                };

                // Attempt to register the business
                var registeredBusiness = await _businessRepository.AddBusinessRegisterAsync(newBusiness);
                if (registeredBusiness == null)
                {
                    _logger.LogWarning("Registration failed: Email {Email} is already in use.", businessRegisterDto.BusEmail);
                    return Conflict(new { error = "This email address is already registered. Please try logging in or use a different email." });
                }

                // Generate an email verification token
                var verificationRecord = await _businessRepository.GenerateEmailVerification(businessRegisterDto.BusEmail);
                string frontendBaseUrl = _configuration["FrontendBaseUrl"];
                if (string.IsNullOrWhiteSpace(frontendBaseUrl))
                {
                    _logger.LogError("Frontend base URL is not configured.");
                    return StatusCode(500, new { message = "Frontend base URL is missing. Please contact support." });
                }

                // Build verification link
                string verificationLink = _verificationLinkBuilder.BuildLink(frontendBaseUrl, verificationRecord.VerificationToken);
                _logger.LogInformation("Generated verification link for {Email}: {VerificationLink}", businessRegisterDto.BusEmail, verificationLink);

                // Send verification email
                await _emailService.SendVerificationEmail(businessRegisterDto.BusEmail, verificationLink);
                _logger.LogInformation("Registration successful for {Email}. Verification email sent.", businessRegisterDto.BusEmail);

                return Ok(new { message = "Registration successful! Please check your email for the verification link." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during registration for {Email}", businessRegisterDto.BusEmail);
                return BadRequest(new { error = "An unexpected error occurred during registration. Please try again later." });
            }
        }

        [HttpGet("verify-business-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var verificationRecord = await _businessRepository.FindVerificationByToken(token);
                if (verificationRecord == null)
                {
                    _logger.LogWarning("Verification token not found: {Token}", token);
                    return BadRequest(new { error = "Invalid verification link." });
                }

                if (verificationRecord.ExpiryDate < DateTime.UtcNow)
                {
                    _logger.LogWarning("Verification token expired: {Token}", token);
                    return BadRequest(new { error = "Verification token has expired. Please request a new one." });
                }

                bool verified = await _businessRepository.VerifyEmail(token);
                if (!verified)
                {
                    _logger.LogWarning("Email verification failed for token: {Token}", token);
                    return BadRequest(new { error = "Email verification failed. Please try again later." });
                }

                _logger.LogInformation("Email verified successfully for token: {Token}", token);
                return Ok(new { message = "Your email has been successfully verified!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during email verification for token: {Token}", token);
                return BadRequest(new { error = "An error occurred during email verification. Please try again later." });
            }
        }

        [HttpPost("resend-business-verification")]
        public async Task<IActionResult> ResendVerificationEmail([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("Resend verification requested without a token.");
                    return BadRequest(new { error = "Verification token is missing." });
                }

                var existingVerification = await _businessRepository.FindVerificationByToken(token);
                if (existingVerification == null)
                {
                    _logger.LogWarning("Resend verification requested: token {Token} not found.", token);
                    return BadRequest(new { error = "Verification token not found. Please register again." });
                }

                string email = existingVerification.Email;
                _logger.LogInformation("Resend verification requested for {Email} (old token: {Token})", email, token);

                // Remove old verification record and generate a new one
                await _businessRepository.RemoveVerification(existingVerification);
                var newVerification = await _businessRepository.GenerateEmailVerification(email);

                string frontendBaseUrl = _configuration["FrontendBaseUrl"];
                if (string.IsNullOrWhiteSpace(frontendBaseUrl))
                {
                    _logger.LogError("Frontend base URL is not configured properly.");
                    return StatusCode(500, new { message = "Frontend base URL is missing. Please contact support." });
                }

                // Build and send new verification link
                string verificationLink = _verificationLinkBuilder.BuildLink(frontendBaseUrl, newVerification.VerificationToken);
                _logger.LogInformation("New verification link generated for {Email}: {VerificationLink}", email, verificationLink);

                await _emailService.SendVerificationEmail(email, verificationLink);
                return Ok(new { message = $"A new verification email has been sent to {email}." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during resend verification for token: {Token}", token);
                return BadRequest(new { error = ex.Message });
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
