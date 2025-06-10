﻿using System.Threading.Tasks;
using mytown.Models;
using mytown.Models.DTO_s;

public interface ICourierServiceRepository
{
    Task<bool> IsCourierEmailTaken(string email);

    Task SavePendingCourierVerification(PendingCourierVerification pending);

    Task<PendingCourierVerification> FindPendingCourierVerificationByToken(string token);

    Task DeletePendingCourierVerification(string token);

    Task<CourierService> RegisterCourier(CourierService courier);
   // Task <List<BestcourierinfoDto>> GetBestCourierOptions(BusinessRegister business, ShopperRegister shopper, decimal productWeightKg);

    Task<List<BestcourierinfoDto>> GetBestCourierOptions(string storeCity, string storeState, string storeCountry, string shopperCity, decimal productWeightKg);
}
