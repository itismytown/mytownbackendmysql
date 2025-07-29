using Microsoft.EntityFrameworkCore;
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
                                    profile_status = bp.profile_status,
                                    bus_time = bp.bus_time,
                                    BusCatId = bp.BusCatId,
                                    BusServId = bp.BusServId,
                                    Businessservice_name = bs != null ? bs.Businessservice_name : null,
                                    Businesscategory_name = bc != null ? bc.Businesscategory_name : null,

                                    // Map Pan as an object
                                    Pan = new PanData
                                    {
                                        X = bp.image_positionx,
                                        Y = bp.image_positiony,
                                        Zoom = bp.zoom
                                    }
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
        public async Task<businessprofile> AddBusinessProfileAsync(businessprofile businessProfile)
        {
            // Check if the business profile with the given BusRegId already exists
            var existingProfile = await _context.BusinessProfiles
                .FirstOrDefaultAsync(bp => bp.BusRegId == businessProfile.BusRegId);

            if (existingProfile != null)
            {
                // Updating an existing profile
                existingProfile.BusinessUsername = businessProfile.BusinessUsername;
                existingProfile.business_location = businessProfile.business_location;
                existingProfile.business_about = businessProfile.business_about;
                existingProfile.banner_path = businessProfile.banner_path;
                existingProfile.profile_status = businessProfile.profile_status;
                existingProfile.bus_time = businessProfile.bus_time;
                existingProfile.BusCatId = businessProfile.BusCatId;
                existingProfile.BusServId = businessProfile.BusServId;

                // Update image position & zoom
                existingProfile.image_positionx = businessProfile.image_positionx;
                existingProfile.image_positiony = businessProfile.image_positiony;
                existingProfile.zoom = businessProfile.zoom;

                // Mark entity as modified
                _context.BusinessProfiles.Update(existingProfile);
            }
            else
            {
                // Set default values if they are not provided
                if (businessProfile.image_positionx == 0 && businessProfile.image_positiony == 0 && businessProfile.zoom == 0)
                {
                    businessProfile.image_positionx = 0;
                    businessProfile.image_positiony = 0;
                    businessProfile.zoom = 1; // Default zoom value
                }

                // Add a new profile
                await _context.BusinessProfiles.AddAsync(businessProfile);
            }

            // Save changes asynchronously
            await _context.SaveChangesAsync();

            // Return the updated or newly added profile
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


    }
}
