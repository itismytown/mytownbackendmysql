namespace mytown.Models
{
    public class PendingCourierVerification
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string JsonPayload { get; set; }
    }
}
