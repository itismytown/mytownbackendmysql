using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class services_sub_categories
    {
        [Key]
        public int serv_subcat_id { get; set; }
        public int BusservId { get; set; }
        [Required]
        [StringLength(100)]
        public string serv_subcat_name { get; set; }
        public string serv_subcat_image { get; set; }

    }
}
