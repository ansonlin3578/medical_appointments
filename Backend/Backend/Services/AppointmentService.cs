using Backend.Models;
using Backend.Repositories;

namespace Backend.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppointmentsRepository _appointmentsRepository;

        public AppointmentService(AppointmentsRepository appointmentsRepository)
        {
            _appointmentsRepository = appointmentsRepository;
        }

        public async Task<ServiceResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            try
            {
                // 檢查時段是否可用
                var timeSlotResult = await IsTimeSlotAvailable(
                    appointment.DoctorId, 
                    appointment.AppointmentDate, 
                    appointment.StartTime, 
                    appointment.EndTime);

                if (!timeSlotResult.Success)
                    return ServiceResult<Appointment>.ErrorResult(timeSlotResult.ErrorMessage, timeSlotResult.ErrorCode);

                appointment.Status = "Scheduled";
                var createdAppointment = await _appointmentsRepository.CreateAsync(appointment);

                return ServiceResult<Appointment>.SuccessResult(createdAppointment);
            }
            catch (Exception ex)
            {
                return ServiceResult<Appointment>.ErrorResult(
                    "An error occurred while creating appointment", 
                    "APPOINTMENT_CREATION_ERROR");
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

                appointment.Status = "Cancelled";
                await _appointmentsRepository.UpdateAsync(appointment);

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