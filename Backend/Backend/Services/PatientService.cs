using Backend.Models;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAppointmentService _appointmentService;

        public PatientService(ApplicationDbContext context, IAppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
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

        public async Task<ServiceResult<Patient>> GetPatientProfile(int patientId)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == patientId);

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

        public async Task<ServiceResult<Patient>> UpdatePatientProfile(int patientId, Patient patient)
        {
            try
            {
                var existingPatient = await _context.Patients.FindAsync(patientId);
                if (existingPatient == null)
                    return ServiceResult<Patient>.ErrorResult("Patient not found", "PATIENT_NOT_FOUND");

                // 更新病人信息
                existingPatient.Name = patient.Name;
                existingPatient.Phone = patient.Phone;
                existingPatient.BirthDate = patient.BirthDate;
                existingPatient.MedicalHistory = patient.MedicalHistory;
                existingPatient.UpdatedAt = DateTime.UtcNow;

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

        public async Task<ServiceResult<IEnumerable<Appointment>>> GetPatientAppointments(int patientId)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Where(a => a.PatientId == patientId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .ToListAsync();

                return ServiceResult<IEnumerable<Appointment>>.SuccessResult(appointments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Appointment>>.ErrorResult(
                    "An error occurred while retrieving patient appointments",
                    "APPOINTMENTS_RETRIEVAL_ERROR");
            }
        }

        public async Task<ServiceResult<bool>> CancelAppointment(int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(appointmentId);
                if (appointment == null)
                    return ServiceResult<bool>.ErrorResult("Appointment not found", "APPOINTMENT_NOT_FOUND");

                // 檢查是否可以取消（例如：預約時間是否在24小時內）
                if (appointment.AppointmentDate <= DateTime.UtcNow.AddHours(24))
                    return ServiceResult<bool>.ErrorResult("Cannot cancel appointment within 24 hours", "CANCELLATION_NOT_ALLOWED");

                appointment.Status = "Cancelled";
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.ErrorResult(
                    "An error occurred while cancelling appointment",
                    "APPOINTMENT_CANCELLATION_ERROR");
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

        public async Task<ServiceResult<bool>> CheckAppointmentConflict(int patientId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                var hasConflict = await _context.Appointments
                    .AnyAsync(a => a.PatientId == patientId &&
                                 a.AppointmentDate.Date == date.Date &&
                                 a.Status != "Cancelled" &&
                                 ((a.StartTime <= startTime && a.EndTime > startTime) ||
                                  (a.StartTime < endTime && a.EndTime >= endTime) ||
                                  (a.StartTime >= startTime && a.EndTime <= endTime)));

                return ServiceResult<bool>.SuccessResult(!hasConflict);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.ErrorResult(
                    "An error occurred while checking appointment conflict",
                    "CONFLICT_CHECK_ERROR");
            }
        }
    }
} 