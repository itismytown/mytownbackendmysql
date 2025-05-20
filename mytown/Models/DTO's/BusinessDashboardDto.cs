namespace mytown.Models.DTO_s
{
    public class BusinessDashboardDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string DeliveryType { get; set; }
        public string DeliveryStatus { get; set; }
    }
}



