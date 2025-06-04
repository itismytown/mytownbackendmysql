using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CourierServiceController : ControllerBase
{
    private readonly ICourierServiceRepository _repository;

    public CourierServiceController(ICourierServiceRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterCourier([FromBody] CourierService courier)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdCourier = await _repository.AddCourierAsync(courier);
        return Ok(createdCourier);
    }
}
