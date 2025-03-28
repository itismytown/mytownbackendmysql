using Microsoft.AspNetCore.Mvc;
using System;
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

    public ShopperController(IShopperRepository shopperRepository, IEmailService emailService)
    {
        _shopperRepository = shopperRepository;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] shopperRegisterDto model)
    {
        try
        {
            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var shopper = new shopperregister
            {
                Username = model.Username,
                Email = model.Email,
                NewPassword = BCrypt.Net.BCrypt.HashPassword(model.Password),
                CnfPassword = BCrypt.Net.BCrypt.HashPassword(model.ConfirmPassword),
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
                return StatusCode(409, new { error = "Email is already in use." }); // Return 409 Conflict


            // var backendUrl = "http://localhost:5100"; // Your actual backend URL
            var verification = await _shopperRepository.GenerateEmailVerification(model.Email);

            var frontendUrl = "https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net"; // Your React app URL
            var verificationLink = $"{frontendUrl}/verify-email?token={verification.VerificationToken}";

            await _emailService.SendVerificationEmail(model.Email, verificationLink);

            Console.WriteLine($"Verification Link: {verificationLink}"); // Debugging


           
          //  var verificationLink = $"{Request.Scheme}://{Request.Host}/api/shopper/verify-email?token={verification.VerificationToken}";

           await _emailService.SendVerificationEmail(model.Email, verificationLink);

            return Ok(new { message = "Registration successful! Please check your email for verification." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        bool isVerified = await _shopperRepository.VerifyEmail(token);

        if (!isVerified)
            return BadRequest("Invalid or expired token.");

        return Ok("Email successfully verified! You can now log in.");
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerificationEmail([FromBody] string email)
    {
        try
        {
            var verification = await _shopperRepository.GenerateEmailVerification(email);
            var verificationLink = $"{Request.Scheme}://{Request.Host}/api/shopper/verify-email?token={verification.VerificationToken}";

            await _emailService.SendVerificationEmail(email, verificationLink);

            return Ok("Verification email resent. Please check your inbox.");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

}

