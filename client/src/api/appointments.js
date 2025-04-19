// api/appointments.js
import axios from 'axios';

export const getAppointments = () =>
  axios.get('/api/appointments').then(res => res.data);

export const getDoctors = () =>
  axios.get('/api/doctors').then(res => res.data);

export const addAppointment = (data) =>
  axios.post('/api/appointments', data);

export const updateAppointment = (id, data) =>
  axios.put(`/api/appointments/${id}`, data);

export const deleteAppointment = (id) =>
  axios.delete(`/api/appointments/${id}`);

export const getOccupiedTimes = (date, doctorId) =>
  axios
    .get('/api/appointments/occupied', {
      params: { date, doctor_id: doctorId }
    })
    .then(res => res.data.occupiedTimes);
