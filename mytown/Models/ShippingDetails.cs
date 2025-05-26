using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    public class ShippingDetails
    {
        [Key]
        public int ShippingDetailId { get; set; } // Primary Key

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required]
        public int OrderDetailId { get; set; } // Foreign Key - OrderDetails

        //[ForeignKey("OrderDetailId")]
        //public virtual orderdetails OrderDetail { get; set; }

        [Required]
        public string Shipping_type { get; set; } // Shipping Courier (e.g., Courier A, Courier B)

        [Required]
        public int EstimatedDays { get; set; } // Estimated Days for Delivery

        [Required]
        public decimal Cost { get; set; } // Shipping Cost

        [Required]
        public string TrackingId { get; set; } // Unique Tracking ID for the product's shipment

        public string ShippingStatus { get; set; } = "Not Shipped";
    }
}
