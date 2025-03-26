using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class services
    {
        [Key]
        public int service_id { get; set; }

        public int BusRegId { get; set; }

        public int BusservId { get; set; }

        public int serv_subcat_id { get; set; }

        [Required]
        [StringLength(100)]
        public string service_name { get; set; }

        public string service_subject { get; set; }

        public string service_description { get; set; }
        public string service_image { get; set; }

        [Range(0, double.MaxValue)]
        public decimal service_cost { get; set; }
    }
}
