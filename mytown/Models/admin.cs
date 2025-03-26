using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class admin
    {
        [Key]
        public int admin_id { get; set; }      

        public string UserEmail { get; set; }

        public string UserPassword { get; set; }
       
    }
}
