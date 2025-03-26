using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class product_sub_categories
    {
        [Key]
        public int prod_subcat_id { get; set; }

        public int BuscatId { get; set; }
        [Required]
        [StringLength(100)]
        public string prod_subcat_name { get; set; }
        public string prod_subcat_image { get; set; }

    }
}
