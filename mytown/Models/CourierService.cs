using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class CourierService
    {
        [Key]
        public int CourierId { get; set; }

        
        [Required]
        public string CourierServiceName { get; set; }
        [Required]
        [StringLength(200)]

        public string CourierAddress { get; set; }
        [Required]
        [StringLength(100)]
        public string CourierTown { get; set; }
        [Required]
        [StringLength(100)]
        public string CourierCity { get; set; }
        [Required]
        [StringLength(100)]
        public string CourierState { get; set; }
        [Required]
        [StringLength(100)]
        public string CourierCountry { get; set; }
        [Required]
        [StringLength(100)]
        public string PostalCode { get; set; }

        [StringLength(15)]
        public string CourierPhone { get; set; }
        [Required]
        [StringLength(100)]
        public string CourierEmail { get; set; }

        [Required]
        public string AadharNumber { get; set; }

        [Required]
        public string LicenseNumber { get; set; }

        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
        public string Password { get; set; }
        public bool IsEmailVerified { get; set; } = false;


    }
}
