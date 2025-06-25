public class CartItemDto
{
    public int CartId { get; set; }
    public int ShopperRegId { get; set; }
    public int product_id { get; set; }
    public int prod_qty { get; set; }
    public string orderstatus { get; set; }
    public string product_name { get; set; }
    public string product_subject { get; set; }
    public string product_description { get; set; }
    public string product_image { get; set; }
    public decimal product_cost { get; set; }

    // New Fields
    public string StoreName { get; set; }
    public string StoreLocation { get; set; }

    public int StoreId { get; set; }
}
