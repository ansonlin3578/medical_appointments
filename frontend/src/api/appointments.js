import axios from 'axios';

const API = axios.create({
  baseURL: process.env.REACT_APP_API_BASE || 'http://localhost:5000/api',
});

export const getAppointments = async () => (await API.get('/appointments')).data;
export const addAppointment = async (data) => (await API.post('/appointments', data)).data;
export const updateAppointment = async (id, data) => (await API.put(`/appointments/${id}`, data)).data;
export const deleteAppointment = async (id) => (await API.delete(`/appointments/${id}`)).data;
export const getDoctors = async () => (await API.get('/doctors')).data;
export const getOccupiedTimes = async (date, doctorId) => {
    const res = await API.get(`/appointments/occupied`, {
      params: { date, doctorId }
    });
    return res.data;
  };