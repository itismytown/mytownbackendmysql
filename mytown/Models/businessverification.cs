using System.ComponentModel.DataAnnotations;


namespace mytown.Models
    {
        public class businessverification
        {
            [Key]
            public int Id { get; set; }
            public int BusRegId { get; set; }
            public string VerificationToken { get; set; }
            public DateTime ExpiryDate { get; set; }
            public bool IsUsed { get; set; }
            public DateTime CreatedAt { get; set; }

            public BusinessRegister Business { get; set; }
        }
    }



