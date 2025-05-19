namespace mytown.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string verificationLink);
        Task SendPasswordResetEmail(string email, string resetLink);
        Task SendBusinessnotification(string email, string businessname, int orderId);

    }
}
