using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class Payments
    {
        [Key]
        public int PaymentId { get; set; } // Primary Key

        [Required]
        public int OrderId { get; set; } // Foreign Key - Orders

        [Required]
        public decimal AmountPaid { get; set; } // Amount Paid

        [Required]
        public string PaymentMethod { get; set; } // Payment Type (Card, UPI, etc.)

        public string PaymentStatus { get; set; } // Pending, Completed

        public DateTime PaymentDate { get; set; } // Date of Payment
    }

}
