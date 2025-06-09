using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    public class CourierBranch
    {
        [Key]
        public int BranchId { get; set; }

        [ForeignKey("CourierService")]
        public int CourierId { get; set; }

        public string CourierName { get; set; } // Redundant but can help for display

        [Required]
        public string Country { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string City { get; set; }

        public string Town { get; set; }

        public string BranchAddress { get; set; }

        public string BranchPhoneNumber { get; set; }

        public string BranchEmailId { get; set; }

        public string BranchContactPerson { get; set; }

        public string Destinations { get; set; } // To be normalized later

        [Required]
        public string ShippingMode { get; set; } // Air / Surface

        [Column(TypeName = "decimal(10,2)")]
        public decimal Charges { get; set; }

        public string WeightRange { get; set; }

        public string DistanceRange { get; set; }

        // 🔗 Navigation
        public CourierService CourierService { get; set; }
    }
}

