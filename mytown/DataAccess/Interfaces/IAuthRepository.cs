namespace mytown.DataAccess.Interfaces
{
    public interface IAuthRepository
    {
        string CreatePasswordResetToken(string email);
        Task SendResetEmail(string email);
        bool ResetPassword(string token,string email, string newPassword);
        bool EmailExists(string email);
    }
}
