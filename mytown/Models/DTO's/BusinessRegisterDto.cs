using System.ComponentModel.DataAnnotations;

namespace mytown.Models
{
    public class BusinessRegisterDto
    {
        public int BusRegId { get; set; }
        public string BusinessUsername { get; set; }
        public string Businessname { get; set; }
        public string LicenseType { get; set; }
        public string Gstin { get; set; }
        public int BusservId { get; set; }
        public int BuscatId { get; set; }
        public string Town { get; set; }
        public string BusMobileNo { get; set; }
        public string BusEmail { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string businessCity { get; set; }
        public string businessState { get; set; }
        public string businessCountry { get; set; }
        public string postalCode { get; set; }
        public DateTime BusinessRegDate { get; set; }
        public string ProfileStatus { get; set; }
        public string Password { get; set; }
        public bool isEmailVerified { get; set; }
    }

}
