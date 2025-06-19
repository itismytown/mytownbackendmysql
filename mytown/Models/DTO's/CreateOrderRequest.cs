namespace mytown.Models.DTO_s
{
    public class CreateOrderRequest
    {
        public int ShopperRegId { get; set; }
        public List<StoreShippingSelection> ShippingSelections { get; set; }
    }
}
