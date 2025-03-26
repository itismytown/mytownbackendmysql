using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class products
    {
        [Key]
        public int product_id {  get; set; }

        public int BusRegId { get; set; }

        public int BuscatId { get; set; }

        public int prod_subcat_id { get; set; }

        [Required]
        [StringLength(100)]
        public string product_name { get; set; }

        [Required]
        [StringLength(100)]
        public string product_subject { get; set; }

        [Required]
        [StringLength(500)]
        public string product_description   { get; set; }
        public string product_image { get; set; }

        [Range(0, double.MaxValue)]
        public decimal product_cost { get; set; }

        [Range(0, double.MaxValue)]
        public decimal product_length { get; set; }

        [Range(0, double.MaxValue)]
        public decimal product_width { get; set; }

        [Range(0, double.MaxValue)]
        public decimal product_weight { get; set; }

        [Range(0, double.MaxValue)]
        public decimal product_quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal product_height { get; set; }

    }
}
