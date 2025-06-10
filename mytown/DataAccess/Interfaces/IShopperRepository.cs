using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IShopperRepository
    {
        Task<ShopperRegister> RegisterShopper(ShopperRegister shopper);
        Task<bool> IsEmailTaken(string email);
        ////     Task<ShopperVerification> GenerateEmailVerification(string email);
        //     Task<bool> VerifyEmail(string token);
        //     Task<ShopperVerification> FindVerificationByToken(string token);
        //     Task RemoveVerification(ShopperVerification verification);
        //     Task SaveVerificationToken(int shopperid, string token, DateTime expirydate);

        Task SavePendingVerification(PendingVerification pending);
        Task<PendingVerification> FindPendingVerificationByToken(string token);
        Task DeletePendingVerification(string token);
        Task<ShopperRegister> GetShopperByIdAsync(int shopperRegId);

    }
}


