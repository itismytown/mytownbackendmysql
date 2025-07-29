using Stripe;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mytown.Models
{
    public class businessprofile
    {
        [Key]
        public int businessprofile_id { get; set; }

        [ForeignKey("BusinessRegister")]
        public int BusRegId { get; set; }
     
        public string BusinessUsername { get; set; }
        public string business_location { get; set; }
        public string business_about { get; set; }
        public string banner_path { get; set; }
        public string logo_path { get; set; }
        public string profile_status { get; set; }
        public string bus_time { get; set; }
        public int BusCatId { get; set; }
        public int BusServId { get; set; }
        public string Businessservice_name { get; set; }
        public string Businesscategory_name { get; set; }

        public DateTime? ApprovedDate { get; set; }

        // Store pan data as separate columns
        public int image_positionx { get; set; }
        public int image_positiony { get; set; }
        public float zoom { get; set; } = 1; // Default value



        public virtual BusinessRegister BusinessRegister { get; set; }
    }

}
