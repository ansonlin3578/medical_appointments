using Microsoft.AspNetCore.Mvc;
using Backend.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private static readonly List<Appointment> _appointments = new();

    [HttpGet]
    public ActionResult<IEnumerable<Appointment>> GetAll()
    {
        return Ok(_appointments);
    }

    [HttpGet("{id}")]
    public ActionResult<Appointment> GetById(Guid id)
    {
        var a = _appointments.FirstOrDefault(x => x.Id == id);
        return a == null ? NotFound() : Ok(a);
    }

    [HttpPost]
    public ActionResult<Appointment> Create(Appointment a)
    {
        a.Id = Guid.NewGuid();
        _appointments.Add(a);
        return CreatedAtAction(nameof(GetById), new { id = a.Id }, a);
    }

    [HttpPut("{id}")]
    public IActionResult Update(Guid id, Appointment updated)
    {
        var a = _appointments.FirstOrDefault(x => x.Id == id);
        if (a == null) return NotFound();

        a.Name = updated.Name;
        a.Date = updated.Date;
        a.StartTime = updated.StartTime;
        a.EndTime = updated.EndTime;
        a.DoctorId = updated.DoctorId;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var a = _appointments.FirstOrDefault(x => x.Id == id);
        if (a == null) return NotFound();

        _appointments.Remove(a);
        return NoContent();
    }
}
