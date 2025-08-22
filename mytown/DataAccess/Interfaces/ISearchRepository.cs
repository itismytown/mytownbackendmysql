﻿using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface ISearchRepository
    {
        // Get list of products based on location and category/subcategory/product search
        List<products> SearchBusinessesWithProducts(string locationQuery, string productQuery);

        // Search for businesses by location and category/product
        Task<List<businessprofile>> SearchBusinessesAsync(string location, string categoryProduct);

        // Get business profiles by location
        List<businessprofile> GetBusinessProfilesByLocation(string location);

        // Get business profiles by product/category search term
        List<businessprofile> GetBusinessProfilesBySearchTerm(string searchTerm);

        // Get business profiles based on both product and location search terms
        List<businessprofile> GetBusinessProfilesByProductAndLocation(string productSearchTerm, string locationSearchTerm);

        //Get product sub categories for that searched town or exitsing in that town
        Task<IEnumerable<product_sub_categories>> GetProductSubCategoriesByLocationAsync(string location);
    }
}
