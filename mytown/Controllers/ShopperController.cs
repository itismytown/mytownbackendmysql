using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using mytown.DataAccess;
using mytown.Models;
using mytown.Services;
using BCrypt.Net;

[Route("api/shopper")]
[ApiController]
public class ShopperController : ControllerBase
{
    private readonly IShopperRepository _shopperRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public ShopperController(IShopperRepository shopperRepository, IEmailService emailService, IConfiguration configuration)
    {
        _shopperRepository = shopperRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] shopperRegisterDto model)
    {
        // Validate input and collect errors
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(model.Username))
            errors.Add("Please enter your name.");

        if (string.IsNullOrWhiteSpace(model.Email))
            errors.Add("Email address is required.");
        else
        {
            // Simple email regex validation.
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(model.Email, emailPattern))
                errors.Add("Please enter a valid email address.");
        }

        if (string.IsNullOrWhiteSpace(model.Password))
            errors.Add("Password is required.");
        else
        {
            if (model.Password.Length < 8)
                errors.Add("Your password must be at least 8 characters long.");
            // Require at least one uppercase, one lowercase, one digit, and one special character.
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            if (!Regex.IsMatch(model.Password, passwordPattern))
                errors.Add("Password must include at least one uppercase letter, one lowercase letter, a number, and a special character.");
        }

        if (model.Password != model.ConfirmPassword)
            errors.Add("The password and confirmation do not match.");

        if (string.IsNullOrWhiteSpace(model.PhoneNo))
            errors.Add("Phone number is required.");

        // Add further validations as needed for Address, Town, City, etc.

        if (errors.Count > 0)
        {
            Console.WriteLine("Registration validation errors:");
            errors.ForEach(e => Console.WriteLine(e));
            return BadRequest(new { errors });
        }

        try
        {
            var shopper = new shopperregister
            {
                Username = model.Username,
                Email = model.Email,
                NewPassword = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Address = model.Address,
                Town = model.Town,
                City = model.City,
                State = model.State,
                Country = model.Country,
                Postalcode = model.Postalcode,
                PhoneNo = model.PhoneNo,
                Photoname = model.Photoname,
                IsEmailVerified = false
            };

            var registeredUser = await _shopperRepository.RegisterShopper(shopper);
            if (registeredUser == null)
            {
                Console.WriteLine($"Registration failed: The email {model.Email} is already in use.");
                return StatusCode(409, new { error = "This email address is already registered. Please try logging in or use a different email." });
            }

            // Generate the email verification token.
            var verification = await _shopperRepository.GenerateEmailVerification(model.Email);

            // Retrieve the frontend URL from configuration.
            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            if (string.IsNullOrEmpty(frontendBaseUrl))
            {
                Console.WriteLine("Error: Frontend base URL is not configured.");
                return StatusCode(500, new { message = "The application configuration is missing the frontend URL. Please contact support." });
            }

            // Construct the verification link, ensuring no double slashes.
            var verificationLink = $"{frontendBaseUrl.TrimEnd('/')}/verify-shopper-email?token={verification.VerificationToken}";
            Console.WriteLine($"Generated verification link: {verificationLink}");

            // Send the verification email.
            await _emailService.SendVerificationEmail(model.Email, verificationLink);
            Console.WriteLine($"Registration successful for {model.Email}. Verification email sent.");

            return Ok(new { message = "Registration successful! Please check your email for the verification link. Once verified, you can log in." });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception during registration: " + ex.ToString());
            return BadRequest(new { error = "An unexpected error occurred during registration. Please try again later." });
        }
    }

    [HttpGet("verify-shopper-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        try
        {
            // Look up the verification record by token.
            var verification = await _shopperRepository.FindVerificationByToken(token);
            if (verification == null)
            {
                Console.WriteLine("Verification token not found: " + token);
                return BadRequest(new { error = "The verification link is invalid. Please contact support." });
            }
            // Check if the token has expired.
            if (verification.ExpiryDate < DateTime.UtcNow)
            {
                Console.WriteLine("Token expired for token: " + token);
                return BadRequest(new { error = "Your verification token has expired. Please request a new verification email." });
            }

            // If token is valid and not expired, proceed with verification.
            bool isVerified = await _shopperRepository.VerifyEmail(token);
            if (!isVerified)
            {
                Console.WriteLine("Verification failed for token: " + token);
                return BadRequest(new { error = "Email verification failed. Please try again later." });
            }
            Console.WriteLine("Email verified successfully for token: " + token);
            return Ok(new { message = "Your email has been successfully verified! You can now log in." });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception during email verification: " + ex.ToString());
            return BadRequest(new { error = "An error occurred during email verification. Please try again later." });
        }
    }


    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerificationEmail([FromQuery] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { error = "Verification token is missing." });

            // Look up the existing verification record by token.
            var verificationRecord = await _shopperRepository.FindVerificationByToken(token);
            if (verificationRecord == null)
                return BadRequest(new { error = "Verification token not found. Please register again or contact support." });

            var email = verificationRecord.Email;
            Console.WriteLine($"Resend requested for email: {email} (old token: {token})");

            // Remove the old verification record.
            await _shopperRepository.RemoveVerification(verificationRecord);

            // Generate a new token for the email.
            var newVerification = await _shopperRepository.GenerateEmailVerification(email);

            // Retrieve the frontend URL from configuration.
            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            if (string.IsNullOrEmpty(frontendBaseUrl))
            {
                Console.WriteLine("Frontend base URL is not configured properly.");
                return StatusCode(500, new { message = "Frontend base URL is not configured properly. Please contact support." });
            }

            // Construct the new verification link.
            var verificationLink = $"{frontendBaseUrl.TrimEnd('/')}/verify-shopper-email?token={newVerification.VerificationToken}";
            Console.WriteLine($"New verification link generated: {verificationLink}");

            // Send the new verification email.
            await _emailService.SendVerificationEmail(email, verificationLink);

            return Ok(new { message = $"A new verification email has been sent to {email}." });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception during resend verification: " + ex.ToString());
            return BadRequest(new { error = ex.Message });
        }
    }

}
