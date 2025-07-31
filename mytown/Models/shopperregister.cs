using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class ShopperRegister
    {
        [Key]
        public int ShopperRegId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public required string Email { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }

        [Required]
        [StringLength(300)]
        public required string Address { get; set; }

        [Required]
        [StringLength(100)]
        public required string Town { get; set; }

        [Required]
        [StringLength(100)]
        public required string City { get; set; }

        [Required]
        [StringLength(100)]
        public required string State { get; set; }

        [Required]
        [StringLength(100)]
        public required string Country { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(15)]
        public required string PhoneNumber { get; set; }

        [StringLength(200)]
        public string PhotoName { get; set; } = string.Empty;

        public string status { get; set; }

        [Required]
        public DateTime ShopperRegDate { get; set; } = DateTime.UtcNow;

    }
}
