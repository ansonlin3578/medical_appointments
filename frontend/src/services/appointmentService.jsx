import axios from 'axios';
import { API_BASE_URL } from '../config';

const appointmentService = {
  getPatientAppointments: async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/patient/appointments`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching appointments:', error);
      throw error;
    }
  },

  getAvailableDoctors: async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/patient/doctors`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching doctors:', error);
      throw error;
    }
  },

  async getAvailableTimeSlots(doctorId, date) {
    try {
      const token = localStorage.getItem('token');
      
      // Format the date as YYYY-MM-DD in UTC
      const formattedDate = new Date(date).toISOString().split('T')[0];
      
      const config = {
        params: { date: formattedDate },
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      };
      
      const response = await axios.get(`${API_BASE_URL}/api/doctor/available-time-slots/${doctorId}`, config);
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
      const token = localStorage.getItem('token');
      
      // Convert the date to UTC
      const utcDate = new Date(appointmentData.appointmentDate);
      const utcDateString = utcDate.toISOString().split('T')[0];
      
      const appointmentPayload = {
        ...appointmentData,
        appointmentDate: utcDateString
      };

      const response = await axios.post(`${API_BASE_URL}/api/appointment`, appointmentPayload, {
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
  },

  async cancelAppointment(appointmentId) {
    try {
      const token = localStorage.getItem('token');
      const response = await axios.post(`${API_BASE_URL}/api/appointment/${appointmentId}/cancel`, null, {
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (response.data && response.data.Message) {
        return response.data.Message;
      }
      return response.data;
    } catch (error) {
      console.error('Error cancelling appointment:', {
        message: error.message,
        response: error.response?.data,
        status: error.response?.status
      });
      throw error;
    }
  }
};

export default appointmentService; 