using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly AppDbContext _context;

        public SearchRepository(AppDbContext context)
        {
            _context = context;
        }

        // API to get list of products based on location and category, subcategory and product search
        public List<products> SearchBusinessesWithProducts(string locationQuery, string productQuery)
        {
            if (string.IsNullOrEmpty(productQuery))
                return new List<products>();

            List<int> filteredBusinesses = new List<int>();

            if (!string.IsNullOrEmpty(locationQuery))
            {
                filteredBusinesses = _context.BusinessRegisters
                    .Where(b =>
                        b.Town.Contains(locationQuery) ||
                        b.businessCity.Contains(locationQuery) ||
                        b.businessState.Contains(locationQuery) ||
                        b.businessCountry.Contains(locationQuery) ||
                        b.Address1.Contains(locationQuery) ||
                        b.Address2.Contains(locationQuery)
                    )
                    .Select(b => b.BusRegId)
                    .ToList();

                if (!filteredBusinesses.Any())
                    return new List<products>();
            }

            var matchingCategories = _context.BusinessCategories
                .Where(c => c.Businesscategory_name.Contains(productQuery))
                .Select(c => c.BuscatId)
                .ToList();

            var matchingSubCategories = _context.product_sub_categories
                .Where(sc => sc.prod_subcat_name.Contains(productQuery))
                .Select(sc => sc.prod_subcat_id)
                .ToList();

            var productsQuery = _context.products.AsQueryable();

            if (!string.IsNullOrEmpty(locationQuery))
            {
                productsQuery = productsQuery.Where(p => filteredBusinesses.Contains(p.BusRegId));
            }

            productsQuery = productsQuery.Where(p =>
                matchingCategories.Contains(p.BuscatId) ||
                matchingSubCategories.Contains(p.prod_subcat_id) ||
                p.product_name.Contains(productQuery) ||
                p.product_subject.Contains(productQuery) ||
                p.product_description.Contains(productQuery)
            );

            return productsQuery.ToList();
        }

        // Search from location and product/category and get the matching store details
        public async Task<List<businessprofile>> SearchBusinessesAsync(string location, string categoryProduct)
        {
            var productResults = new List<products>();
            var busRegIds = new List<int>();
            var businessesByLocation = new List<BusinessRegister>();
            var businessesByCategoryProduct = new List<BusinessRegister>();

            if (!string.IsNullOrEmpty(categoryProduct) && categoryProduct != "null")
            {
                var subCategory = await _context.product_sub_categories
                    .Where(psc => psc.prod_subcat_name.Contains(categoryProduct))
                    .FirstOrDefaultAsync();

                if (subCategory != null)
                {
                    productResults = await _context.products
                        .Where(p => p.prod_subcat_id == subCategory.prod_subcat_id)
                        .ToListAsync();

                    busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
                }
                else
                {
                    var category = await _context.BusinessCategories
                        .Where(c => c.Businesscategory_name.Contains(categoryProduct))
                        .FirstOrDefaultAsync();

                    if (category != null)
                    {
                        productResults = await _context.products
                            .Where(p => p.BuscatId == category.BuscatId)
                            .ToListAsync();

                        busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
                    }
                    else
                    {
                        productResults = await _context.products
                            .Where(p => p.product_name.Contains(categoryProduct) ||
                                        p.product_subject.Contains(categoryProduct) ||
                                        p.product_description.Contains(categoryProduct))
                            .ToListAsync();

                        busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
                    }
                }
            }

            if (!string.IsNullOrEmpty(location) && location != "null")
            {
                businessesByLocation = await _context.BusinessRegisters
                    .Where(br => br.Town.Contains(location) ||
                                 br.businessCity.Contains(location) ||
                                 br.businessState.Contains(location) ||
                                 br.businessCountry.Contains(location))
                    .ToListAsync();
            }

            if (busRegIds.Any())
            {
                businessesByCategoryProduct = await _context.BusinessRegisters
                    .Where(br => busRegIds.Contains(br.BusRegId))
                    .ToListAsync();
            }

            List<BusinessRegister> combinedResults;

            if (!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(categoryProduct) && categoryProduct != "null")
            {
                combinedResults = businessesByLocation
                    .Where(loc => businessesByCategoryProduct.Any(cat => cat.BusRegId == loc.BusRegId))
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(location) && (string.IsNullOrEmpty(categoryProduct) || categoryProduct == "null"))
            {
                combinedResults = businessesByLocation;
            }
            else if (string.IsNullOrEmpty(location) || location == "null")
            {
                combinedResults = businessesByCategoryProduct;
            }
            else
            {
                combinedResults = new List<BusinessRegister>();
            }

            var result = new List<businessprofile>();

            foreach (var business in combinedResults)
            {
                var profile = await _context.BusinessProfiles
                    .FirstOrDefaultAsync(bp => bp.BusRegId == business.BusRegId);

                if (profile != null)
                {
                    result.Add(profile);
                }
            }

            return result;
        }

        // Get profiles based on location search
        public List<businessprofile> GetBusinessProfilesByLocation(string location)
        {
            return _context.BusinessProfiles
                .Where(bp => bp.business_location.Contains(location))
                .ToList();
        }

        // Search for product name in category, subcategory and products tables
        public List<businessprofile> GetBusinessProfilesBySearchTerm(string searchTerm)
        {
            var matchingBusCatIds = _context.BusinessCategories
                .Where(bc => bc.Businesscategory_name.Contains(searchTerm))
                .Select(bc => bc.BuscatId)
                .ToList();

            var businessIds = _context.products
                .Where(p => matchingBusCatIds.Contains(p.BuscatId))
                .Select(p => p.BusRegId)
                .Distinct()
                .ToList();

            if (!businessIds.Any())
            {
                var matchingSubcatIds = _context.product_sub_categories
                    .Where(sc => sc.prod_subcat_name.Contains(searchTerm))
                    .Select(sc => sc.prod_subcat_id)
                    .ToList();

                businessIds = _context.products
                    .Where(p => matchingSubcatIds.Contains(p.prod_subcat_id))
                    .Select(p => p.BusRegId)
                    .Distinct()
                    .ToList();
            }

            if (!businessIds.Any())
            {
                businessIds = _context.products
                    .Where(p =>
                        p.product_name.Contains(searchTerm) ||
                        p.product_subject.Contains(searchTerm) ||
                        p.product_description.Contains(searchTerm))
                    .Select(p => p.BusRegId)
                    .Distinct()
                    .ToList();
            }

            return _context.BusinessProfiles
                .Where(bp => businessIds.Contains(bp.BusRegId))
                .ToList();
        }

        // Fetch Business Profiles based on Product & Location Search Terms
        public List<businessprofile> GetBusinessProfilesByProductAndLocation(string productSearchTerm, string locationSearchTerm)
        {
            var matchingBusCatIds = _context.BusinessCategories
                .Where(bc => bc.Businesscategory_name.Contains(productSearchTerm))
                .Select(bc => bc.BuscatId)
                .ToList();

            var businessIds = _context.products
                .Where(p => matchingBusCatIds.Contains(p.BuscatId))
                .Select(p => p.BusRegId)
                .Distinct()
                .ToList();

            if (!businessIds.Any())
            {
                var matchingSubcatIds = _context.product_sub_categories
                    .Where(sc => sc.prod_subcat_name.Contains(productSearchTerm))
                    .Select(sc => sc.prod_subcat_id)
                    .ToList();

                businessIds = _context.products
                    .Where(p => matchingSubcatIds.Contains(p.prod_subcat_id))
                    .Select(p => p.BusRegId)
                    .Distinct()
                    .ToList();
            }

            if (!businessIds.Any())
            {
                businessIds = _context.products
                    .Where(p =>
                        p.product_name.Contains(productSearchTerm) ||
                        p.product_subject.Contains(productSearchTerm) ||
                        p.product_description.Contains(productSearchTerm))
                    .Select(p => p.BusRegId)
                    .Distinct()
                    .ToList();
            }

            var businessProfiles = _context.BusinessProfiles
                .Where(bp => businessIds.Contains(bp.BusRegId))
                .ToList();

            var filteredProfiles = businessProfiles
                .Where(bp => bp.business_location.Contains(locationSearchTerm))
                .ToList();

            return filteredProfiles;
        }

        public async Task<IEnumerable<product_sub_categories>> GetProductSubCategoriesByLocationAsync(string location)
        {
            // Step 1: Get all business profiles matching location
            var busCatIds = await _context.BusinessProfiles
            .Where(bp => bp.business_location != null &&
                         bp.business_location.Contains(location)) // property name matches model
            .Select(bp => bp.BusCatId)
            .Distinct()
            .ToListAsync();

            if (!busCatIds.Any())
                return new List<product_sub_categories>();

            // Step 2: Get all product_sub_categories for those categories
            var subCategories = await _context.product_sub_categories
                .Where(sc => busCatIds.Contains(sc.BuscatId))
                .Select(sc => new product_sub_categories
                {
                    prod_subcat_id = sc.prod_subcat_id,
                    prod_subcat_name = sc.prod_subcat_name,
                    prod_subcat_image = sc.prod_subcat_image,
                    BuscatId = sc.BuscatId
                })
                .ToListAsync();

            return subCategories;
        }

    }
}
