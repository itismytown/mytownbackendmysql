using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class orderdetails
    {
        [Key]
        public int OrderDetailId { get; set; } // Primary Key

        [Required]
        public int OrderId { get; set; } // Foreign Key - Orders

        [Required]
        public int ProductId { get; set; } // Foreign Key - Products

        [Required]
        public int StoreId { get; set; } // Foreign Key - Store

        [Required]
        public int Quantity { get; set; } // Quantity Ordered

        [Required]
        public decimal Price { get; set; } // Price at Purchase
    }

}
