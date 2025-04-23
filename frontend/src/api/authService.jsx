import axios from 'axios';

const API_URL = 'http://localhost:5000/api';

const authService = {
  async login(username, password) {
    try {
      console.log('Attempting login with:', { username });
      const response = await axios.post(`${API_URL}/auth/login`, {
        username,
        password
      });
      console.log('Login response:', response.data);
      return response.data;
    } catch (error) {
      console.error('Login error:', error.response?.data || error.message);
      throw error;
    }
  },

  async register(username, email, password, role, firstName, lastName) {
    try {
      console.log('Attempting registration with:', { username, email, role });
      const response = await axios.post(`${API_URL}/auth/register`, {
        username,
        email,
        password,
        role,
        firstName,
        lastName
      });
      console.log('Registration response:', response.data);
      return response.data;
    } catch (error) {
      console.error('Registration error:', error.response?.data || error.message);
      throw error;
    }
  },

  async getCurrentUser() {
    const token = localStorage.getItem('token');
    const response = await axios.get(`${API_URL}/auth/me`, {
      headers: {
        Authorization: `Bearer ${token}`
      }
    });
    return response.data;
  },

  logout() {
    localStorage.removeItem('token');
  }
};

export default authService; 