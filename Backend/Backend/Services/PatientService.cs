using Backend.Models;
using Backend.Data;
using Backend.Utils;
using Backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Backend.Constants;

namespace Backend.Services
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<PatientService> _logger;

        public PatientService(ApplicationDbContext context, IAppointmentService appointmentService, ILogger<PatientService> logger)
        {
            _context = context;
            _appointmentService = appointmentService;
            _logger = logger;
        }

        public async Task<ServiceResult<Patient>> CreatePatient(Patient patient)
        {
            try
            {
                // 檢查用戶是否存在
                var user = await _context.Users.FindAsync(patient.UserId);
                if (user == null)
                    return ServiceResult<Patient>.ErrorResult("User not found", "USER_NOT_FOUND");

                // 檢查用戶是否已經是病人
                var existingPatient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == patient.UserId);
                if (existingPatient != null)
                    return ServiceResult<Patient>.ErrorResult("Patient profile already exists", "PATIENT_EXISTS");

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                return ServiceResult<Patient>.SuccessResult(patient);
            }
            catch (Exception ex)
            {
                return ServiceResult<Patient>.ErrorResult(
                    "An error occurred while creating patient profile",
                    "PATIENT_CREATION_ERROR");
            }
        }

        public async Task<ServiceResult<Patient>> GetPatientProfile(int userId)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (patient == null)
                    return ServiceResult<Patient>.ErrorResult("Patient not found", "PATIENT_NOT_FOUND");

                return ServiceResult<Patient>.SuccessResult(patient);
            }
            catch (Exception ex)
            {
                return ServiceResult<Patient>.ErrorResult(
                    "An error occurred while retrieving patient profile",
                    "PATIENT_RETRIEVAL_ERROR");
            }
        }

        public async Task<ServiceResult<Patient>> UpdatePatientProfile(int userId, UpdatePatientDto patientDto)
        {
            try
            {
                var existingPatient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (existingPatient == null || existingPatient.User == null)
                    return ServiceResult<Patient>.ErrorResult("Patient not found", "PATIENT_NOT_FOUND");

                // 更新病人信息
                existingPatient.Name = patientDto.Name;
                // 確保 BirthDate 是 UTC 格式
                existingPatient.BirthDate = patientDto.BirthDate.HasValue 
                    ? DateTime.SpecifyKind(patientDto.BirthDate.Value, DateTimeKind.Utc)
                    : null;
                existingPatient.MedicalHistory = patientDto.MedicalHistory;
                existingPatient.UpdatedAt = DateTime.UtcNow;
                // 更新用户信息
                existingPatient.User.Phone = patientDto.Phone;
                existingPatient.User.Address = patientDto.Address;

                await _context.SaveChangesAsync();

                return ServiceResult<Patient>.SuccessResult(existingPatient);
            }
            catch (Exception ex)
            {
                return ServiceResult<Patient>.ErrorResult(
                    "An error occurred while updating patient profile",
                    "PATIENT_UPDATE_ERROR");
            }
        }

        public async Task<ServiceResult<IEnumerable<Appointment>>> GetPatientAppointments(int userId)
        {
            try
            {   
                _logger.LogInformation($"GetPatientAppointments called with userId: {userId}");
                
                // 首先檢查患者是否存在
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient == null)
                {
                    _logger.LogWarning($"Patient with UserId {userId} not found");
                    return ServiceResult<IEnumerable<Appointment>>.ErrorResult("Patient not found", "PATIENT_NOT_FOUND");
                }
                _logger.LogInformation($"Found patient: {patient.Name}");

                var appointments = await _context.Appointments
                    .Where(a => a.PatientId == userId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .Select(a => new Appointment(
                        a.Id,
                        a.PatientId,
                        a.DoctorId,
                        a.AppointmentDate,
                        a.StartTime,
                        a.EndTime,
                        a.Status
                    ))
                    .ToListAsync();
                
                _logger.LogInformation($"Found {appointments.Count} appointments for patient {patient.Id}");
                return ServiceResult<IEnumerable<Appointment>>.SuccessResult(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetPatientAppointments: {ex.Message}");
                return ServiceResult<IEnumerable<Appointment>>.ErrorResult(
                    "An error occurred while retrieving patient appointments",
                    "PATIENT_APPOINTMENTS_RETRIEVAL_ERROR");
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
                        appointment.StartTime < schedule.EndTime && appointment.EndTime > schedule.StartTime
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

        public async Task<ServiceResult<IEnumerable<User>>> GetAllDoctors()
        {
            try
            {
                var doctors = await _context.Users
                    .Where(u => u.Role == "Doctor")
                    .ToListAsync();

                return ServiceResult<IEnumerable<User>>.SuccessResult(doctors);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<User>>.ErrorResult($"Error retrieving doctors: {ex.Message}");
            }
        }

        public async Task<Patient> GetPatientByUserId(int userId)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Patient> CreatePatientFromUser(User user)
        {
            var patient = new Patient(
                id: 0, // Will be set by the database
                userId: user.Id,
                name: $"{user.FirstName} {user.LastName}"
            );
            patient.User = user;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }
    }
} 