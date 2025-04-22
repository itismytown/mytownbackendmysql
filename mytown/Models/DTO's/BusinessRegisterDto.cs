using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class BusinessRegisterDto
    {
        [Required]
        [StringLength(100)]
        public string BusinessUsername { get; set; }

        [Required]
        [StringLength(100)]
        public string Businessname { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseType { get; set; }

        [StringLength(15)]
        public string Gstin { get; set; }

        [Required]
        public int BusservId { get; set; }

        [Required]
        public int BuscatId { get; set; }

        [Required]
        [StringLength(100)]
        public string Town { get; set; }

        [Required]
        [StringLength(15)]
        public string BusMobileNo { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string BusEmail { get; set; }

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

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100)]
        public string Password { get; set; }

       
    }
}
