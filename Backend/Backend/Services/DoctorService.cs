using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;

        public DoctorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<DoctorSchedule>> SetSchedule(DoctorSchedule schedule)
        {
            try
            {
                // 檢查醫生是否存在
                var doctor = await _context.Users.FindAsync(schedule.DoctorId);
                if (doctor == null || doctor.Role != "Doctor")
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

                if (schedule.Id == 0)
                {
                    _context.DoctorSchedules.Add(schedule);
                }
                else
                {
                    _context.DoctorSchedules.Update(schedule);
                }

                await _context.SaveChangesAsync();
                return ServiceResult<DoctorSchedule>.SuccessResult(schedule);
            }
            catch (Exception ex)
            {
                return ServiceResult<DoctorSchedule>.ErrorResult(
                    "An error occurred while setting schedule",
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
                if (doctor == null || doctor.Role != "Doctor")
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
                    .FirstOrDefaultAsync(u => u.Id == doctorId && u.Role == "Doctor");

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
                if (existingDoctor == null || existingDoctor.Role != "Doctor")
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
                    .Where(u => u.Role == "Doctor")
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
                    .Where(u => u.Role == "Doctor")
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
                    .Include(a => a.Patient)
                    .Where(a => a.DoctorId == doctorId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .ToListAsync();

                return ServiceResult<IEnumerable<Appointment>>.SuccessResult(appointments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Appointment>>.ErrorResult(
                    "An error occurred while retrieving doctor appointments",
                    "APPOINTMENTS_RETRIEVAL_ERROR");
            }
        }

        public async Task<ServiceResult<IEnumerable<DoctorSchedule>>> GetAvailableTimeSlots(int doctorId, DateTime date)
        {
            try
            {
                var dayOfWeek = date.DayOfWeek;
                var schedules = await _context.DoctorSchedules
                    .Where(ds => ds.DoctorId == doctorId && 
                                ds.DayOfWeek == dayOfWeek && 
                                ds.IsAvailable)
                    .OrderBy(ds => ds.StartTime)
                    .ToListAsync();

                // 獲取已預約的時間段
                var appointments = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId && 
                               a.AppointmentDate.Date == date.Date && 
                               a.Status != "Cancelled")
                    .Select(a => new { a.StartTime, a.EndTime })
                    .ToListAsync();

                // 過濾掉已預約的時間段
                var availableSlots = schedules.Where(schedule =>
                    !appointments.Any(appointment =>
                        (schedule.StartTime >= appointment.StartTime && schedule.StartTime < appointment.EndTime) ||
                        (schedule.EndTime > appointment.StartTime && schedule.EndTime <= appointment.EndTime) ||
                        (schedule.StartTime <= appointment.StartTime && schedule.EndTime >= appointment.EndTime)
                    )
                ).ToList();

                return ServiceResult<IEnumerable<DoctorSchedule>>.SuccessResult(availableSlots);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<DoctorSchedule>>.ErrorResult(
                    "An error occurred while retrieving available time slots",
                    "TIME_SLOTS_RETRIEVAL_ERROR");
            }
        }
    }
} 