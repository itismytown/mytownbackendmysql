using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class businesscategoriescs
    {
        [Key]
        public int BuscatId { get; set; } // Primary key

        

        [Required]
        [StringLength(100)]
        public string Businesscategory_name { get; set; }
    }
}
