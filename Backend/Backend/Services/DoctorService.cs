using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Backend.Utils;
using Backend.Constants;

namespace Backend.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(ApplicationDbContext context, ILogger<DoctorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<DoctorSchedule>> SetSchedule(DoctorSchedule schedule)
        {
            try
            {
                // 檢查醫生是否存在
                var doctor = await _context.Users.FindAsync(schedule.DoctorId);
                if (doctor == null || doctor.Role != Roles.Doctor)
                    return ServiceResult<DoctorSchedule>.ErrorResult("Doctor not found", "DOCTOR_NOT_FOUND");

                // 檢查時間是否有效
                if (schedule.StartTime >= schedule.EndTime)
                    return ServiceResult<DoctorSchedule>.ErrorResult("Invalid time range", "INVALID_TIME_RANGE");

                // 檢查是否有重疊的排班
                var overlappingSchedule = await _context.DoctorSchedules
                    .AnyAsync(ds => ds.DoctorId == schedule.DoctorId &&
                                   ds.DayOfWeek == schedule.DayOfWeek &&
                                   ds.Id != schedule.Id &&
                                   ((ds.StartTime <= schedule.StartTime && ds.EndTime > schedule.StartTime) ||
                                    (ds.StartTime < schedule.EndTime && ds.EndTime >= schedule.EndTime) ||
                                    (ds.StartTime >= schedule.StartTime && ds.EndTime <= schedule.EndTime)));

                if (overlappingSchedule)
                    return ServiceResult<DoctorSchedule>.ErrorResult("Schedule overlaps with existing schedule", "SCHEDULE_OVERLAP");

                // Create a new DoctorSchedule without the Doctor navigation property
                var newSchedule = new DoctorSchedule
                {
                    DoctorId = schedule.DoctorId,
                    DayOfWeek = schedule.DayOfWeek,
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime,
                    IsAvailable = schedule.IsAvailable,
                    Notes = schedule.Notes
                };

                if (schedule.Id == 0)
                {
                    _context.DoctorSchedules.Add(newSchedule);
                }
                else
                {
                    newSchedule.Id = schedule.Id;
                    _context.DoctorSchedules.Update(newSchedule);
                }

                await _context.SaveChangesAsync();
                return ServiceResult<DoctorSchedule>.SuccessResult(new DoctorSchedule
                {
                    Id = newSchedule.Id,
                    DoctorId = newSchedule.DoctorId,
                    DayOfWeek = newSchedule.DayOfWeek,
                    StartTime = newSchedule.StartTime,
                    EndTime = newSchedule.EndTime,
                    IsAvailable = newSchedule.IsAvailable,
                    Notes = newSchedule.Notes
                });
            }
            catch (Exception ex)
            {
                return ServiceResult<DoctorSchedule>.ErrorResult(
                    $"An error occurred while setting schedule: {ex.Message}",
                    "SCHEDULE_SET_ERROR");
            }
        }

        public async Task<ServiceResult<IEnumerable<DoctorSchedule>>> GetDoctorSchedules(int doctorId)
        {
            try
            {
                var schedules = await _context.DoctorSchedules
                    .Where(ds => ds.DoctorId == doctorId)
                    .OrderBy(ds => ds.DayOfWeek)
                    .ThenBy(ds => ds.StartTime)
                    .ToListAsync();

                return ServiceResult<IEnumerable<DoctorSchedule>>.SuccessResult(schedules);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<DoctorSchedule>>.ErrorResult(
                    "An error occurred while retrieving schedules",
                    "SCHEDULES_RETRIEVAL_ERROR");
            }
        }

        public async Task<ServiceResult<bool>> DeleteSchedule(int scheduleId)
        {
            try
            {
                var schedule = await _context.DoctorSchedules.FindAsync(scheduleId);
                if (schedule == null)
                    return ServiceResult<bool>.ErrorResult("Schedule not found", "SCHEDULE_NOT_FOUND");

                _context.DoctorSchedules.Remove(schedule);
                await _context.SaveChangesAsync();

                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.ErrorResult(
                    "An error occurred while deleting schedule",
                    "SCHEDULE_DELETION_ERROR");
            }
        }

        public async Task<ServiceResult<DoctorSpecialty>> AddSpecialty(DoctorSpecialty specialty)
        {
            try
            {
                // 檢查醫生是否存在
                var doctor = await _context.Users.FindAsync(specialty.DoctorId);
                if (doctor == null || doctor.Role != Roles.Doctor)
                    return ServiceResult<DoctorSpecialty>.ErrorResult("Doctor not found", "DOCTOR_NOT_FOUND");

                _context.DoctorSpecialties.Add(specialty);
                await _context.SaveChangesAsync();

                return ServiceResult<DoctorSpecialty>.SuccessResult(specialty);
            }
            catch (Exception ex)
            {
                return ServiceResult<DoctorSpecialty>.ErrorResult(
                    "An error occurred while adding specialty",
                    "SPECIALTY_ADDITION_ERROR");
            }
        }

        public async Task<ServiceResult<IEnumerable<DoctorSpecialty>>> GetDoctorSpecialties(int doctorId)
        {
            try
            {
                var specialties = await _context.DoctorSpecialties
                    .Where(ds => ds.DoctorId == doctorId)
                    .OrderBy(ds => ds.Specialty)
                    .ToListAsync();

                return ServiceResult<IEnumerable<DoctorSpecialty>>.SuccessResult(specialties);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<DoctorSpecialty>>.ErrorResult(
                    "An error occurred while retrieving specialties",
                    "SPECIALTIES_RETRIEVAL_ERROR");
            }
        }

        public async Task<ServiceResult<bool>> RemoveSpecialty(int specialtyId)
        {
            try
            {
                var specialty = await _context.DoctorSpecialties.FindAsync(specialtyId);
                if (specialty == null)
                    return ServiceResult<bool>.ErrorResult("Specialty not found", "SPECIALTY_NOT_FOUND");

                _context.DoctorSpecialties.Remove(specialty);
                await _context.SaveChangesAsync();

                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.ErrorResult(
                    "An error occurred while removing specialty",
                    "SPECIALTY_REMOVAL_ERROR");
            }
        }

        public async Task<ServiceResult<User>> GetDoctorProfile(int doctorId)
        {
            try
            {
                var doctor = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == doctorId && u.Role == Roles.Doctor);

                if (doctor == null)
                    return ServiceResult<User>.ErrorResult("Doctor not found", "DOCTOR_NOT_FOUND");

                return ServiceResult<User>.SuccessResult(doctor);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.ErrorResult(
                    "An error occurred while retrieving doctor profile",
                    "DOCTOR_RETRIEVAL_ERROR");
            }
        }

        public async Task<ServiceResult<User>> UpdateDoctorProfile(int doctorId, Doctor doctor)
        {
            try
            {
                var existingDoctor = await _context.Users.FindAsync(doctorId);
                if (existingDoctor == null || existingDoctor.Role != Roles.Doctor)
                    return ServiceResult<User>.ErrorResult("Doctor not found", "DOCTOR_NOT_FOUND");

                // 更新醫生信息
                existingDoctor.FirstName = doctor.Name.Split(' ')[0];
                existingDoctor.LastName = doctor.Name.Split(' ').Length > 1 ? doctor.Name.Split(' ')[1] : "";
                existingDoctor.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ServiceResult<User>.SuccessResult(existingDoctor);
            }
            catch (Exception ex)
            {
                return ServiceResult<User>.ErrorResult(
                    "An error occurred while updating doctor profile",
                    "DOCTOR_UPDATE_ERROR");
            }
        }

        public async Task<ServiceResult<IEnumerable<User>>> SearchDoctors(string specialty, string name)
        {
            try
            {
                var query = _context.Users
                    .Where(u => u.Role == Roles.Doctor)
                    .Include(u => u.DoctorSpecialties)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(specialty))
                {
                    query = query.Where(u => u.DoctorSpecialties.Any(ds => ds.Specialty.Contains(specialty)));
                }

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(u => (u.FirstName + " " + u.LastName).Contains(name));
                }

                var doctors = await query.ToListAsync();
                return ServiceResult<IEnumerable<User>>.SuccessResult(doctors);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<User>>.ErrorResult(
                    "An error occurred while searching doctors",
                    "DOCTOR_SEARCH_ERROR");
            }
        }

        public async Task<ServiceResult<IEnumerable<User>>> GetAvailableDoctors(DateTime date, TimeSpan time)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek;
                var availableDoctors = await _context.Users
                    .Where(u => u.Role == Roles.Doctor)
                    .Include(u => u.DoctorSchedules)
                    .Where(u => u.DoctorSchedules.Any(ds => 
                        ds.DayOfWeek == dayOfWeek &&
                        ds.IsAvailable &&
                        ds.StartTime <= time &&
                        ds.EndTime > time))
                    .ToListAsync();

                return ServiceResult<IEnumerable<User>>.SuccessResult(availableDoctors);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<User>>.ErrorResult(
                    "An error occurred while retrieving available doctors",
                    "AVAILABLE_DOCTORS_RETRIEVAL_ERROR");
            }
        }

        public async Task<ServiceResult<IEnumerable<Appointment>>> GetDoctorAppointments(int doctorId)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .ToListAsync();

                return ServiceResult<IEnumerable<Appointment>>.SuccessResult(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor appointments");
                return ServiceResult<IEnumerable<Appointment>>.ErrorResult(
                    "An error occurred while retrieving doctor appointments",
                    "APPOINTMENTS_RETRIEVAL_ERROR");
            }
        }

        public async Task<ServiceResult<IEnumerable<TimeSlot>>> GetAvailableTimeSlots(int doctorId, DateTime date)
        {
            try
            {
                _logger.LogInformation($"GetAvailableTimeSlots called with doctorId: {doctorId}, date: {date}");

                // First check if the doctor exists
                var doctor = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == doctorId && u.Role == Roles.Doctor);

                if (doctor == null)
                {
                    _logger.LogError($"Doctor not found with ID: {doctorId}");
                    return ServiceResult<IEnumerable<TimeSlot>>.ErrorResult("Doctor not found", "DOCTOR_NOT_FOUND");
                }

                // Get the doctor's schedules for the given day
                var schedules = await _context.DoctorSchedules
                    .Where(s => s.DoctorId == doctorId && s.DayOfWeek == date.DayOfWeek)
                    .ToListAsync();

                if (!schedules.Any())
                {
                    _logger.LogWarning($"No schedule found for doctor {doctorId} on {date.DayOfWeek}");
                    return ServiceResult<IEnumerable<TimeSlot>>.ErrorResult("No schedule found for this day", "NO_SCHEDULE");
                }

                // Get existing appointments for the day
                var appointments = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
                    .Select(a => new { a.StartTime, a.EndTime })
                    .ToListAsync();

                var timeSlots = new List<TimeSlot>();
                foreach (var schedule in schedules)
                {
                    var currentTime = schedule.StartTime;
                    var endTime = schedule.EndTime;

                    while (currentTime < endTime)
                    {
                        var slotEndTime = currentTime.Add(TimeSpan.FromMinutes(30));
                        var isAvailable = !appointments.Any(a =>
                            (a.StartTime <= currentTime && a.EndTime > currentTime) ||
                            (a.StartTime < slotEndTime && a.EndTime >= slotEndTime) ||
                            (a.StartTime >= currentTime && a.EndTime <= slotEndTime));

                        timeSlots.Add(new TimeSlot
                        {
                            Time = currentTime,
                            IsAvailable = isAvailable && schedule.IsAvailable
                        });

                        currentTime = slotEndTime;
                    }
                }

                _logger.LogInformation($"Generated {timeSlots.Count} time slots for doctor {doctorId} on {date}");
                return ServiceResult<IEnumerable<TimeSlot>>.SuccessResult(timeSlots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetAvailableTimeSlots for doctor {doctorId} on {date}");
                return ServiceResult<IEnumerable<TimeSlot>>.ErrorResult(
                    "An error occurred while retrieving available time slots",
                    "TIME_SLOTS_RETRIEVAL_ERROR");
            }
        }
    }
} 