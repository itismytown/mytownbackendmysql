using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
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
        public async Task<(IEnumerable<object> Records, int TotalRecords)> GetBusinessRegistersPaginatedAsync(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;

            var totalRecords = await _context.BusinessRegisters.CountAsync();

            var records = await (
                from b in _context.BusinessRegisters
                join bp in _context.BusinessProfiles
                    on b.BusRegId equals bp.BusRegId into bpJoin
                from bp in bpJoin.DefaultIfEmpty()
                select new
                {
                    b.BusRegId,
                    b.BusinessUsername,
                    b.Businessname,
                    b.LicenseType,
                    b.Gstin,
                    b.BusservId,
                    b.BuscatId,
                    b.Town,
                    b.BusMobileNo,
                    b.BusEmail,
                    b.IsEmailVerified,
                    b.Address1,
                    b.Address2,
                    b.businessCity,
                    b.businessState,
                    b.businessCountry,
                    b.postalCode,
                    b.Password,
                    b.BusinessRegDate,

                    ProfileStatus = bp != null && bp.profile_status != null ? bp.profile_status : "pending",
                    bp.approved_date,

                    ServiceType =
                        b.BusservId == 1 && b.BuscatId == 1 ? "product, service" :
                        b.BuscatId == 1 ? "product" :
                        b.BusservId == 1 ? "service" : "none"
                }
            )
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();

            return (records, totalRecords);
        }


        //get business services

        public async Task<(List<BusinessRegister> Records, int TotalRecords)> GetBusinessesstoresByStatusPaginatedAsync(string status, int page, int pageSize)
        {
            var query = from br in _context.BusinessRegisters
                        join bp in _context.BusinessProfiles
                            on br.BusRegId equals bp.BusRegId into bpGroup
                        from bp in bpGroup.DefaultIfEmpty() // Left join
                        where
                            br.BuscatId == 1 && // Filter by productcategory
                            (
                                (bp != null && bp.profile_status.ToLower() == status.ToLower()) ||
                                (bp == null && status.ToLower() == "incomplete")
                            )
                        select br;

            int totalRecords = await query.CountAsync();

            var records = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }

        //Business summary count for profile status
        public async Task<Dictionary<string, int>> Businessprofilestatuscounts()
        {
            var allStatuses = new[] { "incomplete", "submitted", "approved", "rejected", "blocked" };

            var counts = await _context.BusinessProfiles
                .GroupBy(bp => bp.profile_status.ToLower())
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var finalCounts = allStatuses
                .ToDictionary(
                    status => status,
                    status => counts.FirstOrDefault(c => c.Status == status)?.Count ?? 0
                );

            return finalCounts;
        }

        // Admin  - Approve, Reject, Block business profiles

        public async Task<bool> UpdateProfileStatusbyAdminAsync(int busRegId, string status)
        {
            var profile = await _context.BusinessProfiles
                .FirstOrDefaultAsync(p => p.BusRegId == busRegId);

            if (profile == null)
                return false;

            profile.profile_status = status;
            profile.approved_date = status.ToLower() == "approved" ? DateTime.Now : profile.approved_date;

            _context.BusinessProfiles.Update(profile);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<(List<BusinessRegister> Records, int TotalRecords)> GetBusinessesservicesByStatusPaginated(string status, int page, int pageSize)
        {
            var query = from br in _context.BusinessRegisters
                        join bp in _context.BusinessProfiles
                            on br.BusRegId equals bp.BusRegId into bpGroup
                        from bp in bpGroup.DefaultIfEmpty() // Left join
                        where
                            br.BusservId == 1 && // Filter by servicecategory
                            (
                                (bp != null && bp.profile_status.ToLower() == status.ToLower()) ||
                                (bp == null && status.ToLower() == "incomplete")
                            )
                        select br;

            int totalRecords = await query.CountAsync();

            var records = await query
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

        // Shoppers tab
        public async Task<(IEnumerable<ShopperRegister> records, int totalRecords)> GetShopperRegistersPaginatedAsync(int page, int pageSize)
        {
            var totalRecords = await _context.ShopperRegisters.CountAsync();
            var records = await _context.ShopperRegisters
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalRecords);
        }
        public async Task<bool> UpdateShopperStatusAsync(int shopperId, string newStatus)
        {
            var shopper = await _context.ShopperRegisters.FindAsync(shopperId);
            if (shopper == null)
                return false;

            shopper.status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ShopperRegister?> GetShopperByIdAsync(int shopperId)
        {
            return await _context.ShopperRegisters
                                 .FirstOrDefaultAsync(s => s.ShopperRegId == shopperId);
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


        // landing page
        public async Task<List<LocationStoresDto>> GetLocationsWithCompletedStoresAsync()
        {
            // 1. Get all pending profiles from DB
            var pendingProfiles = await _context.BusinessProfiles
                .Where(bp => bp.profile_status.ToLower() == "incomplete")
                .ToListAsync(); // Materialize here!

            // 2. Group and process in memory
            var result = pendingProfiles
                .GroupBy(bp => bp.business_location.Trim())
                .Where(g => g.Count() >= 3)
                .Select(g =>
                {
                    var parts = g.Key.Split(',').Select(p => p.Trim()).ToArray();

                    var town = parts.Length > 0 ? parts[0] : "";
                    var city = parts.Length > 1 ? parts[1] : "";
                    var country = parts.Length > 3 ? parts[3] : "";

                    // Clean join, skips empty values
                    var locationDisplay = string.Join(", ", new[] { town, city, country }.Where(x => !string.IsNullOrWhiteSpace(x)));

                    return new LocationStoresDto
                    {
                        Location = locationDisplay,
                        Stores = g.ToList()
                    };
                })
                .ToList();

            return result;
        }


    }
}
