using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Repositories;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentsRepository _repo;

    public AppointmentsController(AppointmentsRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAll()
    {
        var data = await _repo.GetAllAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetById(Guid id)
    {
        var a = await _repo.GetByIdAsync(id);
        return a == null ? NotFound() : Ok(a);
    }

    [HttpPost]
    public async Task<ActionResult<Appointment>> Create(Appointment a)
    {
        await _repo.CreateAsync(a);
        return CreatedAtAction(nameof(GetById), new { id = a.Id }, a);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Appointment updated)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        updated.Id = id;
        await _repo.UpdateAsync(updated);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();

        await _repo.DeleteAsync(id);
        return NoContent();
    }
}
