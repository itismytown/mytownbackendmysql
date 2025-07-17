using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace mytown.Models
    {
        public class BusinessVerification
        {
            [Key]
            public int Id { get; set; }
        [ForeignKey("Business")]
        public int BusRegId { get; set; }

            public string VerificationToken { get; set; }
            public DateTime ExpiryDate { get; set; }
            public bool IsUsed { get; set; }
            public DateTime CreatedAt { get; set; }

            public BusinessRegister Business { get; set; }
        }
    }



