namespace mytown.DataAccess.Interfaces
{
    public interface IOrderRepository
    {
        Task<(int OrderId, string TrackingId)> CreateOrderAsync(int shopperRegId, string shippingType);
    }
}

