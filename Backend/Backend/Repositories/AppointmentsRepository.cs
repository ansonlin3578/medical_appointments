using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.Data;

namespace Backend.Repositories;

public class AppointmentsRepository : IAppointmentsRepository
{
    private readonly ApplicationDbContext _context;

    public AppointmentsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task<Appointment> GetByIdAsync(int id)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
    {
        return await _context.Appointments
            .Where(a => a.PatientId == patientId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
    {
        return await _context.Appointments
            .Where(a => a.DoctorId == doctorId)
            .ToListAsync();
    }

    public async Task<Appointment> UpdateAsync(Appointment appointment)
    {
        _context.Entry(appointment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return appointment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
            return false;

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime)
    {
        return !await _context.Appointments
            .AnyAsync(a => a.DoctorId == doctorId &&
                         a.AppointmentDate.Date == date.Date &&
                         a.Status != "Cancelled" &&
                         a.StartTime < endTime && a.EndTime > startTime);
    }
}
