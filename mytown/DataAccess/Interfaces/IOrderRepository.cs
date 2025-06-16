namespace mytown.DataAccess.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderAsync(int shopperRegId, string shippingType, int branchid, decimal cost);
    }
}

