using static System.Runtime.InteropServices.JavaScript.JSType;

using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class BusinessRegister
    {
        [Key]
        public int BusRegId { get; set; } // Primary key

        //[Required]
        //public int RegId { get; set; } // Foreign key to the Registration table

        [Required]
        [StringLength(100)]
        public string BusinessUsername { get; set; }
        [Required]
        [StringLength(100)]
        public string Businessname { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseType { get; set; } // Corrected casing for clarity

        [StringLength(15)]
        public string Gstin { get; set; } // Assuming GSTIN has a specific format

        [Required]
        public int BusservId { get; set; }
        [Required]
        public int BuscatId { get; set; }

        // Uncomment and define if needed
        // [DataType(DataType.Date)]
        // public DateTime Dob { get; set; } // Date of birth or similar, if applicable

        [Required]
        [StringLength(100)]
        public string Town { get; set; }

        [Required]
        [StringLength(15)]
        public string BusMobileNo { get; set; }
        [Required]
        [StringLength(100)]
        public string? BusEmail { get; set; }// Adjusted casing for clarity


        public bool IsEmailVerified { get; set; } = false;

        [Required]
        [StringLength(200)]
        public string Address1 { get; set; }

        [StringLength(200)]
        public string Address2 { get; set; }

        [Required]
        [StringLength(100)]
        public string businessCity { get; set; }

        [Required]
        [StringLength(100)]
        public string businessState { get; set; }

        [Required]
        [StringLength(100)]
        public string businessCountry { get; set; }
        
        [StringLength(10)]
        public string? postalCode { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)] // Password must be at least 6 characters
        public string? Password { get; set; }

        //[Required]
        //[DataType(DataType.Password)]
        //[StringLength(100, MinimumLength = 6)] // Password must be at least 6 characters
        //public string CnfPassword { get; set; }
    }
}

