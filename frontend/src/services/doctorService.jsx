import axios from 'axios';
import { API_BASE_URL } from '../config';

const doctorService = {
  getDoctors: async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/patient/doctors`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching doctors:', error);
      throw error;
    }
  },

  getDoctorProfile: async (doctorId) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/doctor/profile/${doctorId}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching doctor profile:', error);
      throw error;
    }
  },

  getDoctorSpecialties: async (doctorId) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/doctor/specialties/${doctorId}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching doctor specialties:', error);
      throw error;
    }
  },

  updateSpecialty: async (specialtyId, specialtyData) => {
    try {
      const response = await axios.put(`${API_BASE_URL}/api/doctor/specialty/${specialtyId}`, specialtyData, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error updating specialty:', error);
      throw error;
    }
  },

  removeSpecialty: async (specialtyId) => {
    try {
      const response = await axios.delete(`${API_BASE_URL}/api/doctor/specialty/${specialtyId}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error removing specialty:', error);
      throw error;
    }
  },

  addSpecialty: async (specialtyData) => {
    try {
      const response = await axios.post(`${API_BASE_URL}/api/doctor/specialty`, specialtyData, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error adding specialty:', error);
      throw error;
    }
  },

  updateDoctorProfile: async (doctorId, profileData) => {
    try {
      const response = await axios.put(`${API_BASE_URL}/api/doctor/profile/${doctorId}`, profileData, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error updating doctor profile:', error);
      throw error;
    }
  },

  getDoctorSchedule: async (doctorId) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/patient/schedules/${doctorId}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching doctor schedule:', error);
      throw error;
    }
  },

  getAvailableTimeSlots: async (doctorId, date) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/doctor/available-time-slots/${doctorId}`, {
        params: { date },
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching available time slots:', error);
      throw error;
    }
  }
};

export default doctorService; 