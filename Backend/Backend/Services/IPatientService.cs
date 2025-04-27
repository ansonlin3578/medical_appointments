using Backend.Models;
using Backend.Utils;
using Backend.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IPatientService
    {
        Task<ServiceResult<Patient>> CreatePatient(Patient patient);
        Task<ServiceResult<Patient>> GetPatientProfile(int patientId);
        Task<ServiceResult<Patient>> UpdatePatientProfile(int userId, UpdatePatientDto patientDto);
        Task<ServiceResult<IEnumerable<Appointment>>> GetPatientAppointments(int patientId);
        Task<ServiceResult<IEnumerable<DoctorSchedule>>> GetAvailableTimeSlots(int doctorId, DateTime date);
        Task<ServiceResult<IEnumerable<User>>> GetAllDoctors();
        Task<Patient> GetPatientByUserId(int userId);
        Task<Patient> CreatePatientFromUser(User user);
    }
} 