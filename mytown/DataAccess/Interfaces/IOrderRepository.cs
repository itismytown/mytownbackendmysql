using mytown.Models.DTO_s;

namespace mytown.DataAccess.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderAsync(int shopperRegId, List<StoreShippingSelection> shippingSelections);
    }
}

