using Backend.Models;
using Backend.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Backend.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppointmentsRepository _appointmentsRepository;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(AppointmentsRepository appointmentsRepository, ILogger<AppointmentService> logger)
        {
            _appointmentsRepository = appointmentsRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            try
            {
                // Validate appointment data
                var validationResult = ValidateAppointment(appointment);
                if (!validationResult.Success)
                {
                    return validationResult;
                }

                // Process appointment times
                var timeProcessingResult = ProcessAppointmentTimes(appointment);
                if (!timeProcessingResult.Success)
                {
                    return timeProcessingResult;
                }

                var timeSlotResult = await IsTimeSlotAvailable(
                    appointment.DoctorId, 
                    appointment.AppointmentDate, 
                    appointment.StartTime, 
                    appointment.EndTime);

                if (!timeSlotResult.Success)
                    return ServiceResult<Appointment>.ErrorResult(timeSlotResult.ErrorMessage, timeSlotResult.ErrorCode);

                if (!timeSlotResult.Data)
                    return ServiceResult<Appointment>.ErrorResult("該時段已被預約", "TIME_SLOT_NOT_AVAILABLE");

                appointment.Status = "Scheduled";
                var createdAppointment = await _appointmentsRepository.CreateAsync(appointment);

                return ServiceResult<Appointment>.SuccessResult(createdAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return ServiceResult<Appointment>.ErrorResult(
                    "An error occurred while creating appointment", 
                    "APPOINTMENT_CREATION_ERROR");
            }
        }

        private ServiceResult<Appointment> ValidateAppointment(Appointment appointment)
        {
            var modelState = new ModelStateDictionary();
            
            if (appointment == null)
            {
                modelState.AddModelError("", "Appointment data is required");
                return ServiceResult<Appointment>.ErrorResult("Invalid appointment data", "INVALID_APPOINTMENT_DATA");
            }

            if (appointment.DoctorId <= 0)
                modelState.AddModelError("DoctorId", "Invalid doctor ID");

            if (appointment.PatientId <= 0)
                modelState.AddModelError("PatientId", "Invalid patient ID");

            if (appointment.AppointmentDate < DateTime.UtcNow.Date)
                modelState.AddModelError("AppointmentDate", "Appointment date cannot be in the past");

            if (modelState.ErrorCount > 0)
            {
                var errors = modelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                _logger.LogWarning($"Validation errors: {string.Join(", ", errors)}");
                return ServiceResult<Appointment>.ErrorResult(
                    $"Invalid appointment data: {string.Join(", ", errors)}", 
                    "VALIDATION_ERROR");
            }

            return ServiceResult<Appointment>.SuccessResult(appointment);
        }

        private ServiceResult<Appointment> ProcessAppointmentTimes(Appointment appointment)
        {
            try
            {
                // Ensure all DateTime values are in UTC
                appointment.AppointmentDate = DateTime.SpecifyKind(appointment.AppointmentDate.Date, DateTimeKind.Utc);
                appointment.CreatedAt = DateTime.UtcNow;
                appointment.UpdatedAt = DateTime.UtcNow;

                // Convert string times to TimeSpan
                if (TimeSpan.TryParse(appointment.StartTime.ToString(), out TimeSpan startTime) &&
                    TimeSpan.TryParse(appointment.EndTime.ToString(), out TimeSpan endTime))
                {
                    appointment.StartTime = startTime;
                    appointment.EndTime = endTime;
                }
                else
                {
                    _logger.LogWarning($"Failed to parse times: StartTime={appointment.StartTime}, EndTime={appointment.EndTime}");
                    return ServiceResult<Appointment>.ErrorResult(
                        "Invalid time format. Please use HH:mm:ss format",
                        "INVALID_TIME_FORMAT");
                }

                return ServiceResult<Appointment>.SuccessResult(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing appointment times");
                return ServiceResult<Appointment>.ErrorResult(
                    "Error processing appointment times",
                    "TIME_PROCESSING_ERROR");
            }
        }

        public async Task<ServiceResult<IEnumerable<Appointment>>> GetPatientAppointments(int patientId)
        {
            try
            {
                var appointments = await _appointmentsRepository.GetByPatientIdAsync(patientId);
                return ServiceResult<IEnumerable<Appointment>>.SuccessResult(appointments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Appointment>>.ErrorResult(
                    "An error occurred while retrieving patient appointments",
                    "PATIENT_APPOINTMENTS_RETRIEVAL_ERROR");
            }
        }

        public async Task<ServiceResult<IEnumerable<Appointment>>> GetDoctorAppointments(int doctorId)
        {
            try
            {
                var appointments = await _appointmentsRepository.GetByDoctorIdAsync(doctorId);
                return ServiceResult<IEnumerable<Appointment>>.SuccessResult(appointments);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<Appointment>>.ErrorResult(
                    "An error occurred while retrieving doctor appointments",
                    "DOCTOR_APPOINTMENTS_RETRIEVAL_ERROR");
            }
        }

        public async Task<ServiceResult<bool>> CancelAppointment(int appointmentId)
        {
            try
            {
                var appointment = await _appointmentsRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                    return ServiceResult<bool>.ErrorResult("Appointment not found", "APPOINTMENT_NOT_FOUND");

                var deleted = await _appointmentsRepository.DeleteAsync(appointmentId);
                if (!deleted)
                    return ServiceResult<bool>.ErrorResult("Failed to delete appointment", "APPOINTMENT_DELETION_ERROR");

                return ServiceResult<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.ErrorResult(
                    "An error occurred while cancelling appointment",
                    "APPOINTMENT_CANCELLATION_ERROR");
            }
        }

        public async Task<ServiceResult<bool>> IsTimeSlotAvailable(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                var isAvailable = await _appointmentsRepository.IsTimeSlotAvailableAsync(doctorId, date, startTime, endTime);
                return ServiceResult<bool>.SuccessResult(isAvailable);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.ErrorResult(
                    "An error occurred while checking time slot availability",
                    "TIME_SLOT_CHECK_ERROR");
            }
        }

        public async Task<ServiceResult<Appointment>> UpdateAppointment(int appointmentId, Appointment updatedAppointment)
        {
            try
            {
                var existingAppointment = await _appointmentsRepository.GetByIdAsync(appointmentId);
                if (existingAppointment == null)
                {
                    return ServiceResult<Appointment>.ErrorResult("預約不存在", "APPOINTMENT_NOT_FOUND");
                }

                // 檢查新的時間段是否可用
                var isAvailable = await IsTimeSlotAvailable(
                    updatedAppointment.DoctorId,
                    updatedAppointment.AppointmentDate,
                    updatedAppointment.StartTime,
                    updatedAppointment.EndTime
                );

                if (!isAvailable.Success)
                {
                    return ServiceResult<Appointment>.ErrorResult(isAvailable.ErrorMessage, isAvailable.ErrorCode);
                }

                // 更新預約信息
                existingAppointment.AppointmentDate = updatedAppointment.AppointmentDate;
                existingAppointment.StartTime = updatedAppointment.StartTime;
                existingAppointment.EndTime = updatedAppointment.EndTime;
                existingAppointment.Status = updatedAppointment.Status;

                var updated = await _appointmentsRepository.UpdateAsync(existingAppointment);

                return ServiceResult<Appointment>.SuccessResult(updated);
            }
            catch (Exception ex)
            {
                return ServiceResult<Appointment>.ErrorResult("更新預約時發生錯誤", "UPDATE_ERROR");
            }
        }
    }
} 