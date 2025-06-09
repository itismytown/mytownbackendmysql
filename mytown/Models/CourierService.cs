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
        public string CourierContactName { get; set; }

        [StringLength(15)]
        public string CourierPhone { get; set; }
        [Required]
        [StringLength(100)]
        public string CourierEmail { get; set; }

       
        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
        public string Password { get; set; }
       
        public bool IsEmailVerified { get; set; } = false;

        public bool IsLocal { get; set; } = false;
        public bool IsState { get; set; } = false;
        public bool IsNational { get; set; } = false;
        public bool IsInternational { get; set; } = false;

        public ICollection<CourierBranch> CourierBranches { get; set; }

    }
}
