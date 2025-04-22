using Backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IPatientService
    {
        Task<ServiceResult<Patient>> CreatePatient(Patient patient);
        Task<ServiceResult<Patient>> GetPatientProfile(int patientId);
        Task<ServiceResult<Patient>> UpdatePatientProfile(int patientId, Patient patient);
        Task<ServiceResult<IEnumerable<Appointment>>> GetPatientAppointments(int patientId);
        Task<ServiceResult<bool>> CancelAppointment(int appointmentId);
        Task<ServiceResult<IEnumerable<DoctorSchedule>>> GetAvailableTimeSlots(int doctorId, DateTime date);
        Task<ServiceResult<bool>> CheckAppointmentConflict(int patientId, DateTime date, TimeSpan startTime, TimeSpan endTime);
    }
} 