import axios from 'axios';
import { API_BASE_URL } from '../config';

const patientService = {
  getPatientProfile: async (userId) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/patient/profile/${userId}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching patient profile:', error);
      throw error;
    }
  },

  updatePatientProfile: async (userId, profileData) => {
    try {
      // 只發送 Patient 表的字段
      const patientData = {
        userId: userId,
        name: profileData.name,
        phone: profileData.phone || null,
        birthDate: profileData.birthDate || null,
        medicalHistory: profileData.medicalHistory || null,
        address: profileData.address || null
      };

      const response = await axios.put(
        `${API_BASE_URL}/api/patient/profile/${userId}`,
        patientData,
        {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`,
            'Content-Type': 'application/json'
          }
        }
      );
      return response.data;
    } catch (error) {
      console.error('Error updating patient profile:', error);
      throw error;
    }
  }
};

export default patientService;
