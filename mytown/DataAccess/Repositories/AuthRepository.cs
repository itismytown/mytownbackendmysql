using mytown.Models.mytown.DataAccess;
using mytown.Models;
using mytown.Services;
using static mytown.DataAccess.Repositories.AuthRepository;
using mytown.DataAccess.Interfaces;

namespace mytown.DataAccess.Repositories
{

    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthRepository(AppDbContext context, IEmailService emailService,IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        public bool EmailExists(string email)
        {
            return _context.ShopperRegisters.Any(u => u.Email == email) ||
                   _context.BusinessRegisters.Any(u => u.BusEmail == email);
        }


        public string CreatePasswordResetToken(string email)
        {
            var token = Guid.NewGuid().ToString();
            var expiry = DateTime.UtcNow.AddHours(1);

            var request = new PasswordResetRequest
            {
                Email = email,
                Token = token,
                Expiry = expiry
            };

            _context.PasswordResetRequests.Add(request);
            _context.SaveChanges();

            return token;
        }

        public async Task SendResetEmail(string email)
        {
            if (!EmailExists(email))
                throw new Exception("Email not found.");

            var token = CreatePasswordResetToken(email);
            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            var resetLink = $"{frontendBaseUrl}?reset=1&email={email}&token={token}";
           // var resetLink = $"{frontendBaseUrl}/reset-password?token={token}";
           // var resetLink = $"https://mytown-wa-d8gmezfjg7d7hhdy.canadacentral-01.azurewebsites.net/reset-password?token={token}";

            await _emailService.SendPasswordResetEmail(email, resetLink);
        }


        public bool ResetPassword(string email, string newPassword)
        {
            //var request = _context.PasswordResetRequests
            //    .FirstOrDefault(r => r.Token == token && r.Expiry > DateTime.UtcNow);

            //if (request == null) return false;

            var shopper = _context.ShopperRegisters.FirstOrDefault(s => s.Email == email);
            var business = _context.BusinessRegisters.FirstOrDefault(b => b.BusEmail == email);

            if (shopper != null)
            {
                shopper.Password = HashPassword(newPassword); // Replace with your hashing
            }
            else if (business != null)
            {
                business.Password = HashPassword(newPassword);
            }
            else
            {
                return false;
            }

          //  _context.PasswordResetRequests.Remove(email);
            _context.SaveChanges();

            return true;
        }

        private string HashPassword(string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            
            return hashedPassword; 
        }
    }


}
