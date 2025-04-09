using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mytown.DataAccess;
using mytown.Models;
using mytown.Services;
using mytown.Services.Validation;
using BCrypt.Net;
using System.Text.Json;


namespace mytown.Controllers
{
    [Route("api/shoppers")]
    [ApiController]
    public class ShopperController : ControllerBase
    {
        private readonly IShopperRepository _shopperRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ShopperController> _logger;
        private readonly IShopperRegistrationValidator _registrationValidator;
        private readonly IVerificationLinkBuilder _verificationLinkBuilder;

        public ShopperController(
            IShopperRepository shopperRepository,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<ShopperController> logger,
            IShopperRegistrationValidator registrationValidator,
            IVerificationLinkBuilder verificationLinkBuilder)
        {
            _shopperRepository = shopperRepository;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _registrationValidator = registrationValidator;
            _verificationLinkBuilder = verificationLinkBuilder;
        }

        /// <summary>
        /// Registers a new shopper.
        /// </summary>
        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] ShopperRegisterDto shopperRegisterDto)
        //{
        //    // Validate the registration model using the custom validator
        //    List<string> validationErrors = _registrationValidator.Validate(shopperRegisterDto);
        //    if (validationErrors.Count > 0)
        //    {
        //        _logger.LogWarning("Registration validation errors for {Email}: {Errors}", shopperRegisterDto.Email, validationErrors);
        //        return BadRequest(new { errors = validationErrors });
        //    }

            //try
            //{
            //    // Hash the password from the DTO
            //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(shopperRegisterDto.Password);

            //    // Create a new shopper registration instance with the hashed password
            //    var newShopper = new ShopperRegister
            //    {
            //        Username = shopperRegisterDto.Username,
            //        Email = shopperRegisterDto.Email,
            //        Password = hashedPassword,
            //        Address = shopperRegisterDto.Address,
            //        Town = shopperRegisterDto.Town,
            //        City = shopperRegisterDto.City,
            //        State = shopperRegisterDto.State,
            //        Country = shopperRegisterDto.Country,
            //        PostalCode = shopperRegisterDto.PostalCode,
            //        PhoneNumber = shopperRegisterDto.PhoneNumber,
            //        PhotoName = shopperRegisterDto.PhotoName,
            //        IsEmailVerified = false
            //    };

            //    // Attempt to register the shopper
            //    var registeredShopper = await _shopperRepository.RegisterShopper(newShopper);
            //    if (registeredShopper == null)
            //    {
            //        _logger.LogWarning("Registration failed: Email {Email} is already in use.", shopperRegisterDto.Email);
            //        return Conflict(new { error = "This email address is already registered. Please try logging in or use a different email." });
            //    }

            //    // Generate an email verification token and retrieve the frontend base URL
            //    var verificationRecord = await _shopperRepository.GenerateEmailVerification(shopperRegisterDto.Email);
            //    string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            //    if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            //    {
            //        _logger.LogError("Frontend base URL is not configured.");
            //        return StatusCode(500, new { message = "The application configuration is missing the frontend URL. Please contact support." });
            //    }

            //    // Build the verification link using the provided builder service
            //    string verificationLink = _verificationLinkBuilder.BuildLink(frontendBaseUrl, verificationRecord.VerificationToken);
            //    _logger.LogInformation("Generated verification link for {Email}: {VerificationLink}", shopperRegisterDto.Email, verificationLink);

            //    // Send the verification email
            //    await _emailService.SendVerificationEmail(shopperRegisterDto.Email, verificationLink);
            //    _logger.LogInformation("Registration successful for {Email}. Verification email sent.", shopperRegisterDto.Email);

            //    return Ok(new { message = "Registration successful! Please check your email for the verification link. Once verified, you can log in." });
            //}

            [HttpPost("register")]
            public async Task<IActionResult> Register([FromBody] ShopperRegisterDto shopperRegisterDto)
            {
                List<string> validationErrors = _registrationValidator.Validate(shopperRegisterDto);
                if (validationErrors.Count > 0)
                {
                    _logger.LogWarning("Validation failed for {Email}: {Errors}", shopperRegisterDto.Email, validationErrors);
                    return BadRequest(new { errors = validationErrors });
                }

                try
                {
                    bool emailExists = await _shopperRepository.IsEmailTaken(shopperRegisterDto.Email);
                    if (emailExists)
                    {
                        return Conflict(new { error = "This email is already registered. Try logging in instead." });
                    }

                    // ✅ Generate verification token and save to pending table
                    string verificationToken = Guid.NewGuid().ToString();
                    DateTime expiry = DateTime.UtcNow.AddHours(24);
                    string frontendBaseUrl = _configuration["FrontendBaseUrl"];
                    string verificationLink = _verificationLinkBuilder.BuildLink(frontendBaseUrl, verificationToken);

                    // Serialize the registration DTO
                    string jsonPayload = JsonSerializer.Serialize(shopperRegisterDto);

                    // Save to PendingVerification table
                    var pending = new PendingVerification
                    {
                        Email = shopperRegisterDto.Email,
                        Token = verificationToken,
                        ExpiryDate = expiry,
                        JsonPayload = jsonPayload
                    };
                    await _shopperRepository.SavePendingVerification(pending);

                    // Send verification email
                    await _emailService.SendVerificationEmail(shopperRegisterDto.Email, verificationLink);

                    _logger.LogInformation("Verification email sent to {Email}", shopperRegisterDto.Email);
                    return Ok(new { message = "Verification email sent! Please check your inbox to complete registration." });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during registration for {Email}", shopperRegisterDto.Email);
                    return StatusCode(500, new { error = "Something went wrong. Please try again later." });
                }
            }

            [HttpGet("verify-shopper-email")]
            public async Task<IActionResult> VerifyEmail([FromQuery] string token)
            {
                try
                {
                    var pending = await _shopperRepository.FindPendingVerificationByToken(token);
                    if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
                    {
                        return BadRequest(new { error = "Invalid or expired verification link." });
                    }

                    // Deserialize DTO
                    var shopperDto = JsonSerializer.Deserialize<ShopperRegisterDto>(pending.JsonPayload);
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(shopperDto.Password);

                    var newShopper = new ShopperRegister
                    {
                        Username = shopperDto.Username,
                        Email = shopperDto.Email,
                        Password = hashedPassword,
                        Address = shopperDto.Address,
                        Town = shopperDto.Town,
                        City = shopperDto.City,
                        State = shopperDto.State,
                        Country = shopperDto.Country,
                        PostalCode = shopperDto.PostalCode,
                        PhoneNumber = shopperDto.PhoneNumber,
                        PhotoName = shopperDto.PhotoName,
                        IsEmailVerified = true
                    };

                    await _shopperRepository.RegisterShopper(newShopper);
                    await _shopperRepository.DeletePendingVerification(token); // clean up

                    _logger.LogInformation("Email verified and shopper registered for {Email}", newShopper.Email);
                    return Ok(new { message = "Your email is verified and your account has been created!" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error verifying token: {Token}", token);
                    return StatusCode(500, new { error = "Could not verify email. Please try again later." });
                }
            }


            /// <summary>
            /// Resends the email verification link.
            //    /// </summary>
            //    [HttpPost("resend-verification")]
            //    public async Task<IActionResult> ResendVerificationEmail([FromQuery] string token)
            //    {
            //        try
            //        {
            //            if (string.IsNullOrWhiteSpace(token))
            //            {
            //                _logger.LogWarning("Resend verification requested without a token.");
            //                return BadRequest(new { error = "Verification token is missing." });
            //            }

            //            var existingVerification = await _shopperRepository.FindVerificationByToken(token);
            //            if (existingVerification == null)
            //            {
            //                _logger.LogWarning("Resend verification requested: token {Token} not found.", token);
            //                return BadRequest(new { error = "Verification token not found. Please register again or contact support." });
            //            }

            //            //string email = existingVerification.Email;
            //            //_logger.LogInformation("Resend verification requested for {Email} (old token: {Token})", email, token);

            //            // Remove the old verification record and generate a new one
            //            await _shopperRepository.RemoveVerification(existingVerification);
            //            var newVerification = await _shopperRepository.GenerateEmailVerification(email);

            //            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            //            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            //            {
            //                _logger.LogError("Frontend base URL is not configured properly.");
            //                return StatusCode(500, new { message = "Frontend base URL is not configured properly. Please contact support." });
            //            }

            //            // Build and send the new verification link
            //            string verificationLink = _verificationLinkBuilder.BuildLink(frontendBaseUrl, newVerification.VerificationToken);
            //            _logger.LogInformation("New verification link generated for {Email}: {VerificationLink}", email, verificationLink);

            //            await _emailService.SendVerificationEmail(email, verificationLink);
            //            return Ok(new { message = $"A new verification email has been sent to {email}." });
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogError(ex, "Exception during resend verification for token: {Token}", token);
            //            return BadRequest(new { error = ex.Message });
            //        }
            //    }
        }
    }
