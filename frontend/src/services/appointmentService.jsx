import axios from 'axios';
import authService from '../api/authService';

const API_URL = 'http://localhost:5000/api';

const appointmentService = {
  async getPatientAppointments() {
    try {
      const token = authService.getToken();
      
      const config = {
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      };

      const response = await axios.get(`${API_URL}/patient/appointments`, config);
      return response.data;
    } catch (error) {
      console.error('Error fetching patient appointments:', {
        message: error.message,
        response: error.response?.data,
        status: error.response?.status,
        headers: error.response?.headers,
        config: error.config
      });
      throw error;
    }
  },

  async getAvailableDoctors() {
    try {
      const token = authService.getToken();
      
      const config = {
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      };

      const response = await axios.get(`${API_URL}/patient/doctors`, config);
      return response.data;
    } catch (error) {
      console.error('Error fetching available doctors:', {
        message: error.message,
        response: error.response?.data,
        status: error.response?.status,
        headers: error.response?.headers,
        config: error.config
      });
      throw error;
    }
  },

  async getAvailableTimeSlots(doctorId, date) {
    try {
      const token = authService.getToken();
      
      // Format the date as YYYY-MM-DD in UTC
      const formattedDate = new Date(date).toISOString().split('T')[0];
      
      const config = {
        params: { date: formattedDate },
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      };
      
      const response = await axios.get(`${API_URL}/doctor/available-time-slots/${doctorId}`, config);
      return response.data;
    } catch (error) {
      console.error('Error fetching available time slots:', {
        message: error.message,
        response: error.response?.data,
        status: error.response?.status,
        headers: error.response?.headers,
        config: error.config
      });
      throw error;
    }
  },

  async bookAppointment(appointmentData) {
    try {
      const token = authService.getToken();
      const response = await axios.post(`${API_URL}/appointment`, appointmentData, {
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error booking appointment:', error);
      throw error;
    }
  }
};

export default appointmentService; 