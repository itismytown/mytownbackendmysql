﻿using Microsoft.EntityFrameworkCore;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.mytown.DataAccess;
using static mytown.Models.busprofilepreview;

namespace mytown.DataAccess.Repositories
{
    public class BusinessProfileRepository : IBusinessProfileRepository
    {
        private readonly AppDbContext _context;

        public BusinessProfileRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all business profiles including related BusinessRegister data
        public async Task<IEnumerable<businessprofile>> GetAllBusinessProfilesAsync()
        {
            return await _context.BusinessProfiles
                .Include(bp => bp.BusinessRegister) // eager load BusinessRegister
                .ToListAsync();
        }

        public async Task<List<busprofilepreview>> GetBusinessProfilesByBusRegIdAsync(int busRegId)
        {
            var result = await (from bp in _context.BusinessProfiles
                                join bs in _context.BusinessServices
                                    on bp.BusServId equals bs.BusservId into bsGroup
                                from bs in bsGroup.DefaultIfEmpty()
                                join bc in _context.BusinessCategories
                                    on bp.BusCatId equals bc.BuscatId into bcGroup
                                from bc in bcGroup.DefaultIfEmpty()
                                where bp.BusRegId == busRegId
                                select new busprofilepreview
                                {
                                    businessprofile_id = bp.businessprofile_id,
                                    BusRegId = bp.BusRegId,
                                    BusinessUsername = bp.BusinessUsername,
                                    business_location = bp.business_location,
                                    business_about = bp.business_about,
                                    banner_path = bp.banner_path,
                                    logo_path = bp.logo_path,
                                    profile_status = bp.profile_status,
                                    bus_time = bp.bus_time,
                                    BusCatId = bp.BusCatId,
                                    BusServId = bp.BusServId,
                                    Businessservice_name = bs != null ? bs.Businessservice_name : null,
                                    Businesscategory_name = bc != null ? bc.Businesscategory_name : null,

                                    //// Map Pan as an object
                                    //Pan = new PanData
                                    //{
                                    //    X = bp.image_positionx,
                                    //    Y = bp.image_positiony,
                                    //    Zoom = bp.zoom
                                    //}
                                }).ToListAsync();

            return result;
        }



        //get products for selected category
        public IEnumerable<products> GetProductsByBusRegIdAndSubcatId(int busRegId, int prodSubcatId)
        {
            return _context.products
                           .Where(p => p.BusRegId == busRegId && p.prod_subcat_id == prodSubcatId)
                           .ToList();
        }

        // adding business profile data to DB
        //public async Task<businessprofile> AddBusinessProfileAsync(businessprofile businessProfile)
        //{
        //    // Check if the business profile with the given BusRegId already exists
        //    var existingProfile = await _context.BusinessProfiles
        //        .FirstOrDefaultAsync(bp => bp.BusRegId == businessProfile.BusRegId);

        //    if (existingProfile != null)
        //    {
        //        // Updating an existing profile
        //        existingProfile.BusinessUsername = businessProfile.BusinessUsername;
        //        existingProfile.business_location = businessProfile.business_location;
        //        existingProfile.business_about = businessProfile.business_about;
        //        existingProfile.banner_path = businessProfile.banner_path;
        //        existingProfile.profile_status = businessProfile.profile_status;
        //        existingProfile.bus_time = businessProfile.bus_time;
        //        existingProfile.BusCatId = businessProfile.BusCatId;
        //        existingProfile.BusServId = businessProfile.BusServId;

        //        // Update image position & zoom
        //        existingProfile.image_positionx = businessProfile.image_positionx;
        //        existingProfile.image_positiony = businessProfile.image_positiony;
        //        existingProfile.zoom = businessProfile.zoom;

        //        // Mark entity as modified
        //        _context.BusinessProfiles.Update(existingProfile);
        //    }
        //    else
        //    {
        //        // Set default values if they are not provided
        //        if (businessProfile.image_positionx == 0 && businessProfile.image_positiony == 0 && businessProfile.zoom == 0)
        //        {
        //            businessProfile.image_positionx = 0;
        //            businessProfile.image_positiony = 0;
        //            businessProfile.zoom = 1; // Default zoom value
        //        }

        //        // Add a new profile
        //        await _context.BusinessProfiles.AddAsync(businessProfile);
        //    }

        //    // Save changes asynchronously
        //    await _context.SaveChangesAsync();

        //    // Return the updated or newly added profile
        //    return existingProfile ?? businessProfile;
        //}

        public async Task<businessprofile> AddBusinessProfileAsync(businessprofile businessProfile)
        {
            var existingProfile = await _context.BusinessProfiles
                .FirstOrDefaultAsync(bp => bp.BusRegId == businessProfile.BusRegId);

            if (existingProfile != null)
            {
                // Update only if values are provided
                if (!string.IsNullOrEmpty(businessProfile.BusinessUsername))
                    existingProfile.BusinessUsername = businessProfile.BusinessUsername;

                if (!string.IsNullOrEmpty(businessProfile.business_location))
                    existingProfile.business_location = businessProfile.business_location;

                if (!string.IsNullOrEmpty(businessProfile.business_about))
                    existingProfile.business_about = businessProfile.business_about;

                if (!string.IsNullOrEmpty(businessProfile.banner_path))
                    existingProfile.banner_path = businessProfile.banner_path;

                if (!string.IsNullOrEmpty(businessProfile.logo_path))
                    existingProfile.logo_path = businessProfile.logo_path;

                if (!string.IsNullOrEmpty(businessProfile.profile_status))
                    existingProfile.profile_status = businessProfile.profile_status;

                if (!string.IsNullOrEmpty(businessProfile.bus_time))
                    existingProfile.bus_time = businessProfile.bus_time;

                if (businessProfile.BusCatId != 0)
                    existingProfile.BusCatId = businessProfile.BusCatId;

                if (businessProfile.BusServId != 0)
                    existingProfile.BusServId = businessProfile.BusServId;

                if (!string.IsNullOrEmpty(businessProfile.Businessservice_name))
                    existingProfile.Businessservice_name = businessProfile.Businessservice_name;

                if (!string.IsNullOrEmpty(businessProfile.Businesscategory_name))
                    existingProfile.Businesscategory_name = businessProfile.Businesscategory_name;

                // Update coordinates only if they are different from default
                if (businessProfile.image_positionx != 0)
                    existingProfile.image_positionx = businessProfile.image_positionx;

                if (businessProfile.image_positiony != 0)
                    existingProfile.image_positiony = businessProfile.image_positiony;

                if (businessProfile.zoom != 0)
                    existingProfile.zoom = businessProfile.zoom;

                _context.BusinessProfiles.Update(existingProfile);
            }
            else
            {
                // Add new profile
                await _context.BusinessProfiles.AddAsync(businessProfile);
            }

            await _context.SaveChangesAsync();
            return existingProfile ?? businessProfile;
        }


        public async Task<bool> UpdateBannerPathAsync(int busRegId, string bannerPath)
        {
            var business = await _context.BusinessProfiles
                                         .FirstOrDefaultAsync(b => b.BusRegId == busRegId);

            if (business == null)
                return false; // Business not found

            business.banner_path = bannerPath;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateLogoPathAsync(int busRegId, string logoPath)
        {
            var business = await _context.BusinessProfiles
                                         .FirstOrDefaultAsync(b => b.BusRegId == busRegId);

            if (business == null)
                return false; // Business not found

            business.logo_path = logoPath;
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all product subcategories
        public async Task<IEnumerable<product_sub_categories>> GetAllSubCategoriesAsync()
        {
            return await _context.product_sub_categories.ToListAsync();
        }
        //get categories ofproducts for a businessid
        public List<product_sub_categories> GetProductSubCategoriesByBusRegId(int busRegId)
        {
            var result = (from product in _context.products
                          join subCategory in _context.product_sub_categories
                          on product.prod_subcat_id equals subCategory.prod_subcat_id
                          join subCatImage in _context.Subcategoryimages_Busregids
                          on new { product.BusRegId, ProdSubCatId = subCategory.prod_subcat_id }
                          equals new { subCatImage.BusRegId, ProdSubCatId = subCatImage.Prod_subcat_id }
                          into subCatImageGroup
                          from subCatImage in subCatImageGroup.DefaultIfEmpty()
                          where product.BusRegId == busRegId
                          select new product_sub_categories
                          {
                              prod_subcat_id = subCategory.prod_subcat_id,
                              prod_subcat_name = subCategory.prod_subcat_name,
                              prod_subcat_image = subCatImage != null ? subCatImage.Prod_subcat_image : null
                          })
                          .Distinct()
                          .ToList();

            return result;
        }


        // businessprofiels with discount products
        public async Task<IEnumerable<businessprofile>> GetBusinessProfilesWithDiscountedProductsAsync()
        {
            // Step 1: Get distinct business ids from products having discounts
            var businessIdsWithDiscounts = await _context.products
                .Where(p => p.discount.HasValue && p.discount > 0) // only products with valid discount
                .Select(p => p.BusRegId)
                .Distinct()
                .ToListAsync();

            // Step 2: Fetch business profiles for those business ids
            var businessProfiles = await _context.BusinessProfiles
                .Where(bp => businessIdsWithDiscounts.Contains(bp.BusRegId))
                .ToListAsync();

            return businessProfiles;
        }


    }
}
