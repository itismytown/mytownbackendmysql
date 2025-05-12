namespace mytown.Models
{
    public class ShopperRegisterDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Town { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string PhotoName { get; set; } = string.Empty;
        public DateTime ShopperRegDate { get; set; }

    }
}
