using Microsoft.AspNetCore.Mvc;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private static readonly List<Doctor> _doctors = new()
    {
        new Doctor { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Dr. Lee" },
        new Doctor { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Dr. Wang" },
        new Doctor { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Dr. Chang" }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Doctor>> GetAll()
    {
        return Ok(_doctors);
    }
}
