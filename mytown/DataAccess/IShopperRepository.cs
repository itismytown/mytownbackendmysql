using mytown.Migrations;
using mytown.Models;

namespace mytown.DataAccess
{
    public interface IShopperRepository
    {
        Task<shopperregister> RegisterShopper(shopperregister shopper);
        Task<bool> IsEmailTaken(string email);
        Task<ShopperVerification> GenerateEmailVerification(string email);
        Task<bool> VerifyEmail(string token);
    }
}


