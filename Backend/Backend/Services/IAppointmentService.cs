using Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IAppointmentService
    {
        Task<ServiceResult<Appointment>> CreateAppointment(Appointment appointment);
        Task<ServiceResult<IEnumerable<Appointment>>> GetPatientAppointments(int patientId);
        Task<ServiceResult<IEnumerable<Appointment>>> GetDoctorAppointments(int doctorId);
        Task<ServiceResult<bool>> CancelAppointment(int appointmentId);
        Task<ServiceResult<bool>> IsTimeSlotAvailable(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime);
        Task<ServiceResult<Appointment>> UpdateAppointment(int appointmentId, Appointment updatedAppointment);
    }
} 