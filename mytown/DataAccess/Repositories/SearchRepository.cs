using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mytown.Models;
using mytown.Models.mytown.DataAccess;

namespace mytown.DataAccess.Repositories
{
    //public class SearchRepository : ISearchRepository

    //{
    //    private readonly AppDbContext _context;

    //    public SearchRepository(AppDbContext context)
    //    {
    //        _context = context;
    //    }
    //    //api to get list of products based on location and category, subcategory and prodcut search
    //    public List<products> SearchBusinessesWithProducts(string locationQuery, string productQuery)
    //    {
    //        if (string.IsNullOrEmpty(productQuery))
    //            return new List<products>(); // If no productQuery, return empty list

    //        List<int> filteredBusinesses = new List<int>();

    //        // **Step 1: Filter Businesses by Location if locationQuery is provided**
    //        if (!string.IsNullOrEmpty(locationQuery))
    //        {
    //            filteredBusinesses = _context.BusinessRegisters
    //                .Where(b =>
    //                    b.Town.Contains(locationQuery) ||
    //                    b.businessCity.Contains(locationQuery) ||
    //                    b.businessState.Contains(locationQuery) ||
    //                    b.businessCountry.Contains(locationQuery) ||
    //                    b.Address1.Contains(locationQuery) ||
    //                    b.Address2.Contains(locationQuery)
    //                )
    //                .Select(b => b.BusRegId)
    //                .ToList();

    //            if (!filteredBusinesses.Any())
    //                return new List<products>(); // No matching businesses found for the location
    //        }

    //        // **Step 2: Check if productQuery matches Business Categories**
    //        var matchingCategories = _context.BusinessCategories
    //            .Where(c => c.Businesscategory_name.Contains(productQuery))
    //            .Select(c => c.BuscatId)
    //            .ToList();

    //        // **Step 3: Check if productQuery matches Product Subcategories**
    //        var matchingSubCategories = _context.product_sub_categories
    //            .Where(sc => sc.prod_subcat_name.Contains(productQuery))
    //            .Select(sc => sc.prod_subcat_id)
    //            .ToList();

    //        // **Step 4: Get Products Based on Location and Product Matches**
    //        var productsQuery = _context.products.AsQueryable();

    //        if (!string.IsNullOrEmpty(locationQuery))
    //        {
    //            // **Filter products that belong to businesses in the selected location**
    //            productsQuery = productsQuery.Where(p => filteredBusinesses.Contains(p.BusRegId));
    //        }

    //        // **Apply Product Search Filters**
    //        productsQuery = productsQuery.Where(p =>
    //            matchingCategories.Contains(p.BuscatId) ||
    //            matchingSubCategories.Contains(p.prod_subcat_id) ||
    //            p.product_name.Contains(productQuery) ||
    //            p.product_subject.Contains(productQuery) ||
    //            p.product_description.Contains(productQuery)
    //        );

    //        return productsQuery.ToList();
    //    }


    //    // search from location and prodcut/category and get the matching store details
    //    public async Task<List<businessprofile>> SearchBusinessesAsync(string location, string categoryProduct)
    //    {
    //        var productResults = new List<products>();
    //        var busRegIds = new List<int>();
    //        var businessesByLocation = new List<BusinessRegister>();
    //        var businessesByCategoryProduct = new List<BusinessRegister>();

    //        // Step 1: If categoryProduct is provided, search through ProductSubCategories, BusinessCategories, and Products
    //        if (!string.IsNullOrEmpty(categoryProduct) && categoryProduct != "null")
    //        {
    //            // Search ProductSubCategories
    //            var subCategory = await _context.product_sub_categories
    //                .Where(psc => psc.prod_subcat_name.Contains(categoryProduct))
    //                .FirstOrDefaultAsync();

    //            if (subCategory != null)
    //            {
    //                productResults = await _context.products
    //                    .Where(p => p.prod_subcat_id == subCategory.prod_subcat_id)
    //                    .ToListAsync();

    //                busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
    //            }
    //            else
    //            {
    //                // Search BusinessCategories
    //                var category = await _context.BusinessCategories
    //                    .Where(c => c.Businesscategory_name.Contains(categoryProduct))
    //                    .FirstOrDefaultAsync();

    //                if (category != null)
    //                {
    //                    productResults = await _context.products
    //                        .Where(p => p.BuscatId == category.BuscatId)
    //                        .ToListAsync();

    //                    busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
    //                }
    //                else
    //                {
    //                    // Search directly in Products table if no category or subcategory found
    //                    productResults = await _context.products
    //                        .Where(p => p.product_name.Contains(categoryProduct) || p.product_subject.Contains(categoryProduct) || p.product_description.Contains(categoryProduct))
    //                        .ToListAsync();

    //                    busRegIds.AddRange(productResults.Select(p => p.BusRegId).Distinct());
    //                }
    //            }
    //        }

    //        // Step 2: If location is provided, filter by location in BusinessRegister
    //        if (!string.IsNullOrEmpty(location) && location != "null")
    //        {
    //            businessesByLocation = await _context.BusinessRegisters
    //                .Where(br => br.Town.Contains(location) || br.businessCity.Contains(location) || br.businessState.Contains(location) || br.businessCountry.Contains(location))
    //                .ToListAsync();
    //        }

    //        // Step 3: Filter BusinessRegister by matching BusRegIds for product search
    //        if (busRegIds.Any())
    //        {
    //            businessesByCategoryProduct = await _context.BusinessRegisters
    //                .Where(br => busRegIds.Contains(br.BusRegId))
    //                .ToListAsync();
    //        }

    //        // Step 4: Handle search results based on available criteria

    //        List<BusinessRegister> combinedResults;

    //        if (!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(categoryProduct) && categoryProduct != "null")
    //        {
    //            // If both location and categoryProduct are provided, find intersection
    //            combinedResults = businessesByLocation
    //                .Where(loc => businessesByCategoryProduct
    //                    .Any(cat => cat.BusRegId == loc.BusRegId))
    //                .ToList();
    //        }
    //        else if (!string.IsNullOrEmpty(location) && (string.IsNullOrEmpty(categoryProduct) || categoryProduct == "null"))
    //        {
    //            // If only location is provided, return results matching location
    //            combinedResults = businessesByLocation;
    //        }
    //        else if (string.IsNullOrEmpty(location) || location == "null")
    //        {
    //            // If location is not provided or "null", use categoryProduct to filter
    //            combinedResults = businessesByCategoryProduct;
    //        }
    //        else
    //        {
    //            // Default case if neither location nor categoryProduct is provided
    //            combinedResults = new List<BusinessRegister>();
    //        }

    //        // Step 5: Retrieve BusinessProfile details for each matching BusinessRegister
    //        var result = new List<businessprofile>();

    //        foreach (var business in combinedResults)
    //        {
    //            var profile = await _context.BusinessProfiles
    //                .FirstOrDefaultAsync(bp => bp.BusRegId == business.BusRegId);

    //            if (profile != null)
    //            {
    //                result.Add(profile); // Only add BusinessProfile to result
    //            }
    //        }

    //        return result;
    //    }





    //    //** get location search mathcing on business profiles 

    //    //get profiles based on location search 
    //    public List<businessprofile> GetBusinessProfilesByLocation(string location)
    //    {
    //        return _context.BusinessProfiles
    //    .Where(bp => bp.business_location.Contains(location))
    //    .ToList();
    //    }


    //    //search for prodcut name in category, subcategory and prodcuts tables
    //    public List<businessprofile> GetBusinessProfilesBySearchTerm(string searchTerm)
    //    {
    //        // Step 1: Search in businesscategoriescs table
    //        var matchingBusCatIds = _context.BusinessCategories
    //            .Where(bc => bc.Businesscategory_name.Contains(searchTerm))
    //            .Select(bc => bc.BuscatId)
    //            .ToList();

    //        // Step 2: Use found BuscatIds to search in products table
    //        var businessIds = _context.products
    //            .Where(p => matchingBusCatIds.Contains(p.BuscatId))
    //            .Select(p => p.BusRegId)
    //            .Distinct()
    //            .ToList();

    //        // Step 3: If no match in categories, search in product_sub_categories table
    //        if (!businessIds.Any())
    //        {
    //            var matchingSubcatIds = _context.product_sub_categories
    //                .Where(sc => sc.prod_subcat_name.Contains(searchTerm))
    //                .Select(sc => sc.prod_subcat_id)
    //                .ToList();

    //            businessIds = _context.products
    //                .Where(p => matchingSubcatIds.Contains(p.prod_subcat_id))
    //                .Select(p => p.BusRegId)
    //                .Distinct()
    //                .ToList();
    //        }

    //        // Step 4: If still no match, search directly in products table
    //        if (!businessIds.Any())
    //        {
    //            businessIds = _context.products
    //                .Where(p =>
    //                    p.product_name.Contains(searchTerm) ||
    //                    p.product_subject.Contains(searchTerm) ||
    //                    p.product_description.Contains(searchTerm))
    //                .Select(p => p.BusRegId)
    //                .Distinct()
    //                .ToList();
    //        }

    //        // Step 5: Fetch Full Business Profiles
    //        return _context.BusinessProfiles
    //            .Where(bp => businessIds.Contains(bp.BusRegId))
    //            .ToList();
    //    }

    //    // Fetch Business Profiles based on Product & Location Search Terms
    //    public List<businessprofile> GetBusinessProfilesByProductAndLocation(string productSearchTerm, string locationSearchTerm)
    //    {
    //        // Step 1: Get BusRegIds matching the product search term
    //        var matchingBusCatIds = _context.BusinessCategories
    //            .Where(bc => bc.Businesscategory_name.Contains(productSearchTerm))
    //            .Select(bc => bc.BuscatId)
    //            .ToList();

    //        var businessIds = _context.products
    //            .Where(p => matchingBusCatIds.Contains(p.BuscatId))
    //            .Select(p => p.BusRegId)
    //            .Distinct()
    //            .ToList();

    //        if (!businessIds.Any())
    //        {
    //            var matchingSubcatIds = _context.product_sub_categories
    //                .Where(sc => sc.prod_subcat_name.Contains(productSearchTerm))
    //                .Select(sc => sc.prod_subcat_id)
    //                .ToList();

    //            businessIds = _context.products
    //                .Where(p => matchingSubcatIds.Contains(p.prod_subcat_id))
    //                .Select(p => p.BusRegId)
    //                .Distinct()
    //                .ToList();
    //        }

    //        if (!businessIds.Any())
    //        {
    //            businessIds = _context.products
    //                .Where(p =>
    //                    p.product_name.Contains(productSearchTerm) ||
    //                    p.product_subject.Contains(productSearchTerm) ||
    //                    p.product_description.Contains(productSearchTerm))
    //                .Select(p => p.BusRegId)
    //                .Distinct()
    //                .ToList();
    //        }

    //        // Step 2: Get full business profiles
    //        var businessProfiles = _context.BusinessProfiles
    //            .Where(bp => businessIds.Contains(bp.BusRegId))
    //            .ToList();

    //        // Step 3: Filter profiles by location search term
    //        var filteredProfiles = businessProfiles
    //            .Where(bp => bp.business_location.Contains(locationSearchTerm))
    //            .ToList();

    //        return filteredProfiles;
    //    }

    //}
}
