using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IAuthRepository
    {
        string CreatePasswordResetToken(string email);
        Task SendResetEmail(string email);
        bool ResetPassword(string email, string newPassword);
        bool EmailExists(string email);

        PasswordResetRequest GetResetRequestByToken(string token);
    }
}
