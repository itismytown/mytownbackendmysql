﻿using mytown.Models;
using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IShopperRepository
    {
        Task<ShopperRegister> RegisterShopper(ShopperRegister shopper);
        Task<(bool isTaken, string message)> IsEmailTaken(string email);
        ////     Task<ShopperVerification> GenerateEmailVerification(string email);
        //     Task<bool> VerifyEmail(string token);

        //resend email
             Task<ShopperVerification> FindPendingVerificationByEmail(string email);
            Task RemoveVerification(ShopperVerification verification);
      

     

        Task SavePendingVerification(PendingVerification pending);
        Task<PendingVerification> FindPendingVerificationByToken(string token);
        Task DeletePendingVerification(string token);
        Task<ShopperRegister> GetShopperByIdAsync(int shopperRegId);
        Task<IEnumerable<object>> GetTownsWithStoreCountByCountryAsync(string country);

        Task<IEnumerable<ProductDto>> GetRecentlyViewedProductsAsync(int shopperId, int days = 7, int limit = 10);

    }
}


