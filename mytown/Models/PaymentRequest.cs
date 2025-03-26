namespace mytown.Models
{
    public class PaymentRequest
    {
        public string CountryName { get; set; } // Name of the country
        public string CurrencySymbol { get; set; } // Symbol or code of currency
        public long Amount { get; set; } // Amount in cents (e.g., $10 = 1000)
    }

}
