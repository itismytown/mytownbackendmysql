using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class order
    {
        [Key]
        public int OrderId { get; set; } // Primary Key

        [Required]
        public int ShopperRegId { get; set; } // Foreign Key - Shopper

        [Required]
        public decimal TotalAmount { get; set; } // Order Total

        [Required]
        public string ShippingType { get; set; } // Shipping Type (Standard, Peer-to-Peer, etc.)

        public string OrderStatus { get; set; } // Pending, Shipped, Delivered, etc.

        public DateTime OrderDate { get; set; } // Order Creation Date
    }

}
