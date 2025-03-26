using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class shopperregister
    {
        [Key]
        public int ShopperRegId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)] // Password must be at least 6 characters
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)] // Password must be at least 6 characters
        public string CnfPassword { get; set; }
        [Required]
        [StringLength(300)]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        public string Town { get; set; } // Town

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; } // Town

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        
        [StringLength(10)]
        public string Postalcode { get; set; }
        [Required]
        [Phone]
        [StringLength(15)] // Adjust based on phone number format
        public string PhoneNo { get; set; } // Phone Number


        [StringLength(200)] // Adjust based on phone number format
        public string Photoname { get; set; } // Phone Number

    }
}
