using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface IPaymentRepository
    {
        Payments AddPayment(int orderId, decimal amountPaid, string paymentMethod);
    }
}
