using mytown.Models;

namespace mytown.DataAccess.Interfaces
{
    public interface ICourierServiceRepository
    {
        Task<CourierService> AddCourierAsync(CourierService courier);
    }
}
