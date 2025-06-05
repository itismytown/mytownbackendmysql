using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class CourierServiceRepository : ICourierServiceRepository
    {
        private readonly AppDbContext _context;

        public CourierServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsCourierEmailTaken(string email)
        {
            return await _context.CourierService.AnyAsync(c => c.CourierEmail == email);
        }

        public async Task SavePendingCourierVerification(PendingCourierVerification pending)
        {
            _context.PendingCourierVerifications.Add(pending);
            await _context.SaveChangesAsync();
        }

        public async Task<PendingCourierVerification> FindPendingCourierVerificationByToken(string token)
        {
            return await _context.PendingCourierVerifications.FirstOrDefaultAsync(p => p.Token == token);
        }
        public async Task DeletePendingCourierVerification(string token)
        {
            var record = await _context.PendingCourierVerifications.FirstOrDefaultAsync(p => p.Token == token);
            if (record != null)
            {
                _context.PendingCourierVerifications.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<CourierService> RegisterCourier(CourierService courier)
        {
            _context.CourierService.Add(courier);
            await _context.SaveChangesAsync();
            return courier;
        }



        //    public async Task<CourierService> AddCourierAsync(CourierService courier)
        //{
        //    _context.CourierService.Add(courier);
        //    await _context.SaveChangesAsync();
        //    return courier;
        //}
    }
}
