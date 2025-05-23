using Backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Services
{
    public interface IDoctorService
    {
        // 排班管理
        Task<ServiceResult<DoctorSchedule>> SetSchedule(DoctorSchedule schedule);
        Task<ServiceResult<IEnumerable<DoctorSchedule>>> GetDoctorSchedules(int doctorId);
        Task<ServiceResult<bool>> DeleteSchedule(int scheduleId);
        
        // 專業領域管理
        Task<ServiceResult<DoctorSpecialty>> AddSpecialty(DoctorSpecialty specialty);
        Task<ServiceResult<IEnumerable<DoctorSpecialty>>> GetDoctorSpecialties(int doctorId);
        Task<ServiceResult<bool>> RemoveSpecialty(int specialtyId);
        
        // 醫生信息管理
        Task<ServiceResult<User>> UpdateDoctorProfile(int doctorId, Doctor doctor);
        Task<ServiceResult<User>> GetDoctorProfile(int doctorId);
        
        // 搜索和過濾
        Task<ServiceResult<IEnumerable<User>>> SearchDoctors(string specialty, string name);
        Task<ServiceResult<IEnumerable<User>>> GetAvailableDoctors(DateTime date, TimeSpan time);

        Task<ServiceResult<IEnumerable<Appointment>>> GetDoctorAppointments(int doctorId);
        Task<ServiceResult<IEnumerable<DoctorSchedule>>> GetAvailableTimeSlots(int doctorId, DateTime date);
    }
} 