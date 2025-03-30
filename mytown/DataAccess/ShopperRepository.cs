using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess;
using mytown.Models;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess
{
    public class ShopperRepository : IShopperRepository
    {
        private readonly AppDbContext _context;

        public ShopperRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShopperRegister> RegisterShopper(ShopperRegister shopper)
        {
            if (await IsEmailTaken(shopper.Email))
                return null;
            //throw new Exception("Email is already in use.");
            shopper.ConfirmPassword = shopper.Password;
            shopper.IsEmailVerified = false;

            _context.ShopperRegisters.Add(shopper);
            await _context.SaveChangesAsync();

            return shopper;
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return await _context.ShopperRegisters.AnyAsync(s => s.Email == email);
        }

        public async Task<ShopperVerification> GenerateEmailVerification(string email)
        {
            var shopper = await _context.ShopperRegisters.FirstOrDefaultAsync(s => s.Email == email);
            if (shopper == null) throw new Exception("User not found.");

            var token = Guid.NewGuid().ToString();
            var expiryDate = DateTime.UtcNow.AddHours(24);

            var verification = new ShopperVerification
            {
                Email = email,
                VerificationToken = token,
                ExpiryDate = expiryDate,
                IsVerified = false
            };

            _context.ShopperVerification.Add(verification);
            await _context.SaveChangesAsync();

            return verification;
        }

        public async Task<bool> VerifyEmail(string token)
        {
            var verification = await _context.ShopperVerification.FirstOrDefaultAsync(v => v.VerificationToken == token);

            if (verification == null || verification.ExpiryDate < DateTime.UtcNow)
                return false;

            var shopper = await _context.ShopperRegisters.FirstOrDefaultAsync(s => s.Email == verification.Email);
            if (shopper == null) return false;

            shopper.IsEmailVerified = true;
            _context.ShopperRegisters.Update(shopper);
            _context.ShopperVerification.Remove(verification);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ShopperVerification> FindVerificationByToken(string token)
        {
            return await _context.ShopperVerification.FirstOrDefaultAsync(v => v.VerificationToken == token);
        }

        public async Task RemoveVerification(ShopperVerification verification)
        {
            _context.ShopperVerification.Remove(verification);
            await _context.SaveChangesAsync();
        }



    }
}
