using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services;
using System.Text.Json;

using System.Threading.Tasks;

[ApiController]
[Route("api/courier")]
public class CourierController : ControllerBase
{
    private readonly ICourierServiceRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CourierController> _logger;
    private readonly IVerificationLinkBuildercourier _verificationLinkBuilder;

    public CourierController(ICourierServiceRepository repository,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<CourierController> logger,
        IVerificationLinkBuildercourier verificationLinkBuilder)
    {
        _repository = repository;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
        _verificationLinkBuilder = verificationLinkBuilder;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterCourier([FromBody] CourierServiceDto courierDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            if (await _repository.IsCourierEmailTaken(courierDto.CourierEmail))
            {
                return Conflict(new { error = "Email already registered. Try logging in." });
            }

            // Uncomment for email verification
            /*
            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddHours(24);
            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            string verificationLink = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);

            string jsonPayload = JsonSerializer.Serialize(courierDto);

            var pending = new PendingCourierVerification
            {
                Email = courierDto.Email,
                Token = token,
                ExpiryDate = expiry,
                JsonPayload = jsonPayload
            };

            await _repository.SavePendingVerification(pending);
            await _emailService.SendVerificationEmail(courierDto.Email, verificationLink);
            return Ok(new { message = "Verification email sent! Please check your inbox." });
            */

            // SKIP EMAIL FOR NOW AND REGISTER DIRECTLY
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(courierDto.Password);

            var courier = new CourierService
            {
                CourierServiceName = courierDto.CourierServiceName,
                CourierAddress = courierDto.CourierAddress,
                CourierTown = courierDto.CourierTown,
                CourierCity = courierDto.CourierCity,
                CourierState = courierDto.CourierState,
                CourierCountry = courierDto.CourierCountry,
                PostalCode = courierDto.PostalCode,
                CourierPhone = courierDto.CourierPhone,
                CourierEmail = courierDto.CourierEmail,
                AadharNumber = courierDto.AadharNumber,
                LicenseNumber = courierDto.LicenseNumber,
                Password = hashedPassword,
                IsEmailVerified = true,   
                RegisteredDate = DateTime.UtcNow
            };

            var createdCourier = await _repository.RegisterCourier(courier);
            return Ok(new { message = "Verification link sent to email. Please verify and login.", courier = createdCourier });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering courier for {Email}", courierDto.CourierEmail);
            return StatusCode(500, new { error = "Something went wrong. Please try again." });
        }
    }

    [HttpGet("verify-courier-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        try
        {
            var pending = await _repository.FindPendingCourierVerificationByToken(token);
            if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
            {
                return BadRequest(new { error = "Invalid or expired verification link." });
            }

            var courierDto = JsonSerializer.Deserialize<CourierServiceDto>(pending.JsonPayload);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(courierDto.Password);

            var courier = new CourierService
            {
                CourierServiceName = courierDto.CourierServiceName,
                CourierAddress = courierDto.CourierAddress,
                CourierTown = courierDto.CourierTown,
                CourierCity = courierDto.CourierCity,
                CourierState = courierDto.CourierState,
                CourierCountry = courierDto.CourierCountry,
                PostalCode = courierDto.PostalCode,
                CourierPhone = courierDto.CourierPhone,
                CourierEmail = courierDto.CourierEmail,
                AadharNumber = courierDto.AadharNumber,
                LicenseNumber = courierDto.LicenseNumber,
                Password = hashedPassword,
                IsEmailVerified = true,
                RegisteredDate = DateTime.UtcNow
            };
            await _repository.RegisterCourier(courier);
            await _repository.DeletePendingCourierVerification(token);

            return Ok(new { message = "Courier email verified and registration completed!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying courier token: {Token}", token);
            return StatusCode(500, new { error = "Could not verify email. Try again later." });
        }
    }
}

