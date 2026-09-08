namespace mytown.Models
{
    public class CashfreePayoutApiResponse
    {
        public string? TransferId { get; set; }
        public string? CfTransferId { get; set; }
        public string? Status { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusDescription { get; set; }
        public decimal? TransferAmount { get; set; }
        public string? TransferMode { get; set; }
        public string? TransferUtr { get; set; }
    }
}