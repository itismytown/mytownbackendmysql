﻿namespace mytown.Models
{
    public class ShopperRegisterDto
    {
        public int ShopperRegId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoName { get; set; }
        public string Status { get; set; }
        public string Password { get; set; }
        public DateTime ShopperRegDate { get; set; }
    }
}
