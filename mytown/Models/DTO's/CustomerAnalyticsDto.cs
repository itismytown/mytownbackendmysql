namespace mytown.Models.DTO_s
{
    public class CustomerAnalyticsDto
    {
        public int CustomersVisitedAndPurchased { get; set; }
        public int CustomersVisitedButNotPurchased { get; set; }
        public List<FrequentCustomerDto> FrequentCustomers { get; set; }
        public List<CustomerDto> CustomersWhoPurchased { get; set; }
    }
}

