namespace mytown.Models
{
    public class PasswordResetRequest
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime Expiry { get; set; }
    }
}
