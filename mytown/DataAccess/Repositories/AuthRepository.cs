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

        public AuthRepository(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
            var resetLink = $"https://yourfrontend.com/reset-password?token={token}";

            await _emailService.SendPasswordResetEmail(email, resetLink);
        }


        public bool ResetPassword(string token, string newPassword)
        {
            var request = _context.PasswordResetRequests
                .FirstOrDefault(r => r.Token == token && r.Expiry > DateTime.UtcNow);

            if (request == null) return false;

            // You already have a method like this in your repo:
            var shopper = _context.ShopperRegisters.FirstOrDefault(s => s.Email == request.Email);
            var business = _context.BusinessRegisters.FirstOrDefault(b => b.BusEmail == request.Email);

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

            _context.PasswordResetRequests.Remove(request);
            _context.SaveChanges();

            return true;
        }

        private string HashPassword(string password)
        {
            // Use your preferred hashing method
            return password; // Placeholder
        }
    }


}
