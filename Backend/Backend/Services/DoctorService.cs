using Backend.Data;
using Backend.Models;
using Backend.Utils;
using Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Backend.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                                   ds.StartTime < schedule.EndTime && ds.EndTime > schedule.StartTime);

                if (overlappingSchedule)
                    return ServiceResult<DoctorSchedule>.ErrorResult("Schedule overlaps with existing schedule", "SCHEDULE_OVERLAP");

                // Create a new DoctorSchedule without the Doctor navigation property
                var newSchedule = new DoctorSchedule(
                    id: schedule.Id,
                    doctorId: schedule.DoctorId,
                    dayOfWeek: schedule.DayOfWeek,
                    startTime: schedule.StartTime,
                    endTime: schedule.EndTime,
                    isAvailable: schedule.IsAvailable
                )
                {
                    Notes = schedule.Notes
                };

                if (schedule.Id == 0)
                {
                    _context.DoctorSchedules.Add(newSchedule);
                }
                else
                {
                    _context.DoctorSchedules.Update(newSchedule);
                }

                await _context.SaveChangesAsync();
                return ServiceResult<DoctorSchedule>.SuccessResult(new DoctorSchedule(
                    id: newSchedule.Id,
                    doctorId: newSchedule.DoctorId,
                    dayOfWeek: newSchedule.DayOfWeek,
                    startTime: newSchedule.StartTime,
                    endTime: newSchedule.EndTime,
                    isAvailable: newSchedule.IsAvailable
                )
                {
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

                var newSpecialty = new DoctorSpecialty(
                    id: 0,
                    doctorId: specialty.DoctorId,
                    specialty: specialty.Specialty,
                    description: specialty.Description,
                    yearsOfExperience: specialty.YearsOfExperience
                );

                _context.DoctorSpecialties.Add(newSpecialty);
                await _context.SaveChangesAsync();

                return ServiceResult<DoctorSpecialty>.SuccessResult(newSpecialty);
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
        
        public async Task<ServiceResult<DoctorSpecialty>> UpdateSpecialty(int specialtyId, DoctorSpecialty specialty)
        {
            try
            {
                var existingSpecialty = await _context.DoctorSpecialties.FindAsync(specialtyId);
                if (existingSpecialty == null)
                    return ServiceResult<DoctorSpecialty>.ErrorResult("Specialty not found", "SPECIALTY_NOT_FOUND");

                existingSpecialty.Specialty = specialty.Specialty;
                existingSpecialty.Description = specialty.Description;
                existingSpecialty.YearsOfExperience = specialty.YearsOfExperience;

                await _context.SaveChangesAsync();

                return ServiceResult<DoctorSpecialty>.SuccessResult(existingSpecialty);
            }
            catch (Exception ex)
            {
                return ServiceResult<DoctorSpecialty>.ErrorResult(
                    "An error occurred while updating specialty",
                    "SPECIALTY_UPDATE_ERROR");
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

        public async Task<ServiceResult<User>> UpdateDoctorProfile(int doctorId, UserProfileUpdateDto profileUpdate)
        {
            try
            {
                var existingUser = await GetDoctorProfile(doctorId);
                if (!existingUser.Success || existingUser.Data == null)
                    return existingUser;

                var user = existingUser.Data;
                if (profileUpdate.FirstName != null) user.FirstName = profileUpdate.FirstName;
                if (profileUpdate.LastName != null) user.LastName = profileUpdate.LastName;
                if (profileUpdate.Email != null) user.Email = profileUpdate.Email;
                if (profileUpdate.Phone != null) user.Phone = profileUpdate.Phone;
                if (profileUpdate.Address != null) user.Address = profileUpdate.Address;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return ServiceResult<User>.SuccessResult(user);
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
                var utcDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
                var nextDay = utcDate.AddDays(1);

                var appointments = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId && 
                               a.AppointmentDate >= utcDate &&
                               a.AppointmentDate < nextDay &&
                               a.Status != "Cancelled")
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
                            currentTime < a.EndTime && slotEndTime > a.StartTime);
  
                        timeSlots.Add(new TimeSlot
                        {
                            Time = currentTime,
                            IsAvailable = isAvailable && schedule.IsAvailable
                        });

                        currentTime = slotEndTime;
                    }
                }

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