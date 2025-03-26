namespace mytown.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string email, string verificationLink);
    }
}
