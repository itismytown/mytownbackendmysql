using mytown.Models;

namespace mytown.DataAccess
{
    public interface IShopperRepository
    {
        Task<ShopperRegister> RegisterShopper(ShopperRegister shopper);
        Task<bool> IsEmailTaken(string email);
        Task<ShopperVerification> GenerateEmailVerification(string email);
        Task<bool> VerifyEmail(string token);
        Task<ShopperVerification> FindVerificationByToken(string token);
        Task RemoveVerification(ShopperVerification verification);
    }
}


