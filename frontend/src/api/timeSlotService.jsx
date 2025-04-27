import axios from 'axios';
import authService from './authService';

const API_URL = 'http://localhost:5000/api';

const timeSlotService = {
  async getDoctorTimeSlots(doctorId) {
    try {
      const token = authService.getToken();
      const response = await axios.get(`${API_URL}/doctor/available-time-slots/${doctorId}`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching time slots:', error);
      throw error;
    }
  },

  async getDoctorSchedules(doctorId) {
    try {
      const token = authService.getToken();
      const response = await axios.get(`${API_URL}/doctor/schedules/${doctorId}`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching doctor schedules:', error);
      throw error;
    }
  },

  async createTimeSlot(timeSlotData) {
    try {
      const token = authService.getToken();
      const schedule = {
        doctorId: timeSlotData.doctorId,
        dayOfWeek: timeSlotData.dayOfWeek,
        startTime: timeSlotData.startTime,
        endTime: timeSlotData.endTime,
        isAvailable: timeSlotData.isAvailable,
        notes: timeSlotData.notes || ''
      };
      
      const response = await axios.post(`${API_URL}/doctor/schedule`, schedule, {
        headers: {
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error creating time slot:', error);
      throw error;
    }
  },

  async deleteTimeSlot(slotId) {
    try {
      const token = authService.getToken();
      await axios.delete(`${API_URL}/doctor/schedule/${slotId}`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
    } catch (error) {
      console.error('Error deleting time slot:', error);
      throw error;
    }
  }
};

export default timeSlotService; 