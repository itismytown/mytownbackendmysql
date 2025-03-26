using System;
using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class ShopperVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }

        [Required]
        public string VerificationToken { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsVerified { get; set; } = false;
    }
}
