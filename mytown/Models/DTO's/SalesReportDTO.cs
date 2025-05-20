namespace mytown.Models.DTO_s
{
    public class SalesReportDTO
    {
        public decimal TotalSales { get; set; }
        public int TotalProductsSold { get; set; }
        public int UniqueOrdersCount { get; set; }
        public int UniqueShoppersCount { get; set; }
    }
}
