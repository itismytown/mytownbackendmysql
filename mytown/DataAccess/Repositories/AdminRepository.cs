using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        //ADMIN PANEL

        //to get all business profiles with status
        public async Task<(IEnumerable<BusinessRegister> records, int totalRecords)> GetBusinessRegistersPaginatedAsync(int page, int pageSize)
        {
            var totalRecords = await _context.BusinessRegisters.CountAsync();
            var records = await _context.BusinessRegisters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }




        public async Task<(int uniqueCities, int uniqueStates, int uniqueCountries)> GetUniqueCountsAsync()
        {
            // Fetch unique cities from both tables
            var uniqueCities = await _context.BusinessRegisters
                .Select(b => b.businessCity)
                .Where(city => !string.IsNullOrEmpty(city))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.City)
                        .Where(city => !string.IsNullOrEmpty(city))
                )
                .Distinct()
                .CountAsync();

            // Fetch unique states from both tables
            var uniqueStates = await _context.BusinessRegisters
                .Select(b => b.businessState)
                .Where(state => !string.IsNullOrEmpty(state))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.State)
                        .Where(state => !string.IsNullOrEmpty(state))
                )
                .Distinct()
                .CountAsync();

            // Fetch unique countries from both tables
            var uniqueCountries = await _context.BusinessRegisters
                .Select(b => b.businessCountry)
                .Where(country => !string.IsNullOrEmpty(country))
                .Union(
                    _context.ShopperRegisters
                        .Select(s => s.Country)
                        .Where(country => !string.IsNullOrEmpty(country))
                )
                .Distinct()
                .CountAsync();

            return (uniqueCities, uniqueStates, uniqueCountries);
        }

        public async Task<int> GetBusinessRegisterCountAsync()
        {
            // Count the rows in the BusinessRegister table
            int count = await _context.BusinessRegisters.CountAsync();
            return count;
        }

        public async Task<(IEnumerable<ShopperRegister> records, int totalRecords)> GetShopperRegistersPaginatedAsync(int page, int pageSize)
        {
            var totalRecords = await _context.ShopperRegisters.CountAsync();
            var records = await _context.ShopperRegisters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }

        public async Task<int> GetShoppersRegisterCountAsync()
        {
            // Count the rows in the BusinessRegister table
            int count = await _context.ShopperRegisters.CountAsync();
            return count;
        }

        public async Task<int> GetCourierserviceCountAsync()
        {
            // Count the rows in the BusinessRegister table
            int count = await _context.CourierService.CountAsync();
            return count;
        }

        public async Task<(IEnumerable<CourierService> records, int totalRecords)> GetCourierRegistersPaginatedAsync(int page, int pageSize)
        {
            var totalRecords = await _context.CourierService.CountAsync();
            var records = await _context.CourierService
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }
    }
}
