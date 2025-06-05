using System.Threading.Tasks;
using mytown.Models;

public interface ICourierServiceRepository
{
    Task<bool> IsCourierEmailTaken(string email);

    Task SavePendingCourierVerification(PendingCourierVerification pending);

    Task<PendingCourierVerification> FindPendingCourierVerificationByToken(string token);

    Task DeletePendingCourierVerification(string token);

    Task<CourierService> RegisterCourier(CourierService courier);
}
