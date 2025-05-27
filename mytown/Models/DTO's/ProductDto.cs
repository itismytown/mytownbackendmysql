namespace mytown.Models.DTO_s
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public int ProductType { get; set; }
        public string ProductName { get; set; }
        public decimal ProductAmount { get; set; }
        public decimal Quantity { get; set; }
        public int PurchasedCount { get; set; }
        public string ProductImage { get; set; }
    }
}
