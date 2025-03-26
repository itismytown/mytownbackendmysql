using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class subcategoryimages_busregid
    {
        [Key]
        public int  Image_Id { get; set; }
        public int BusRegId { get; set; }
      

        public int BuscatId { get; set; }
        public int Prod_subcat_id { get; set; }
       
        [StringLength(100)]
        public string Prod_subcat_name { get; set; }
        public string Prod_subcat_image { get; set; }
    }
}
