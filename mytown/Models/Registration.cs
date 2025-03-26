using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class Registration
    {
        [Key]
        public int RegId { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)] // Password must be at least 6 characters
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)] // Password must be at least 6 characters
        public string CnfPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Dob { get; set; } // Date of Birth

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

        [Required]
        [Phone]
        [StringLength(15)] // Adjust based on phone number format
        public string PhoneNo { get; set; } // Phone Number

        [StringLength(6)]
        public string Otp { get; set; } // One-time Password

        [Required]
        [StringLength(50)]
        public string Role { get; set; } // User Role (e.g., Admin, User)
    }
    }
