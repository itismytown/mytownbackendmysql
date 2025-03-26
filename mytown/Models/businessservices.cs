using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class businessservices
    {
        [Key]
        public int BusservId { get; set; } // Primary key



        [Required]
        [StringLength(100)]
        public string Businessservice_name { get; set; }
    }
}
