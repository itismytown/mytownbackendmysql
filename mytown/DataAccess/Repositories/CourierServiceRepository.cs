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

        public async Task<CourierService> AddCourierAsync(CourierService courier)
        {
            _context.CourierService.Add(courier);
            await _context.SaveChangesAsync();
            return courier;
        }
    }
}
