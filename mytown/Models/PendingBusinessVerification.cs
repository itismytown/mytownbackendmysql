using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class PendingBusinessVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string JsonPayload { get; set; }
    }
}
