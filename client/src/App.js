import React, { useEffect, useState } from 'react';
import AppointmentForm from './components/AppointmentForm';
import AppointmentList from './components/AppointmentList';
import {
  getAppointments,
  getDoctors,
  addAppointment,
  updateAppointment,
  deleteAppointment,
  getOccupiedTimes
} from './api/appointments';

function App() {
  const [appointments, setAppointments] = useState([]);
  const [form, setForm] = useState({ name: '', date: '', start_time: '', end_time: '', doctor_id: '' });
  const [doctors, setDoctors] = useState([]);
  const today = new Date().toISOString().split('T')[0];
  const [occupiedTimes, setOccupiedTimes] = useState([]);
  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({ date: '', start_time: '', end_time: '', doctor_id: '' });
  const [editOccupiedTimes, setEditOccupiedTimes] = useState([]);



  const fetchAppointments = async () => {
    try {
      const data = await getAppointments();
      setAppointments(data);
    } catch (err) {
      console.error('Error fetching appointments:', err);
    }
  };

  const fetchDoctors = async () => {
    try {
      const data = await getDoctors();
      setDoctors(data);
    } catch (err) {
      console.error('Error fetching doctors:', err);
    }
  };
  

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.name || !form.date || !form.start_time || !form.end_time || !form.doctor_id) {
      alert('請輸入完整資料（姓名、日期、時段、醫生）');
      return;
    }
  
    try {
      await addAppointment(form);
      setForm({ name: '', date: '', start_time: '', end_time: '', doctor_id: '' });
      fetchAppointments();
    } catch (err) {
      const msg = err.response?.data?.error || '預約失敗，請稍後再試';
      alert(msg);
    }
  };
  

  const handleUpdate = async (id) => {
    try {
      await updateAppointment(id, editForm);
      setEditingId(null);
      fetchAppointments();
    } catch (err) {
      alert(err.response?.data?.error || '更新失敗');
    }
  };
  
  
  const handleDelete = async (id) => {
    try {
      await deleteAppointment(id);
      fetchAppointments();
    } catch (err) {
      console.error('Error deleting appointment:', err);
    }
  };
  

  useEffect(() => {
    fetchDoctors();
    fetchAppointments();
  }, []);

  useEffect(() => {
    const fetchOccupiedTimes = async () => {
      if (!form.date || !form.doctor_id) {
        setOccupiedTimes([]);
        return;
      }
    
      try {
        const times = await getOccupiedTimes(form.date, form.doctor_id);
        setOccupiedTimes(times);
      } catch (err) {
        console.error('Error fetching occupied times:', err);
      }
    };
  
    fetchOccupiedTimes();
  }, [form.date, form.doctor_id]);

  useEffect(() => {
    const fetchEditOccupied = async () => {
      if (!editingId || !editForm.date || !editForm.doctor_id) {
        setEditOccupiedTimes([]);
        return;
      }
    
      try {
        const times = await getOccupiedTimes(editForm.date, editForm.doctor_id);
        setEditOccupiedTimes(times.filter(t => t !== editForm.start_time));
      } catch (err) {
        console.error('Error fetching edit occupied times:', err);
      }
    };
  
    fetchEditOccupied();
  }, [editingId, editForm.date, editForm.doctor_id, editForm.start_time]);
  

  return (
    <div style={{ padding: 20 }}>
      <h2>預約看診</h2>
      <AppointmentForm
        form={form}
        setForm={setForm}
        doctors={doctors}
        occupiedTimes={occupiedTimes}
        onSubmit={handleSubmit}
        today={today}
      />

      <h3>預約列表</h3>
      <AppointmentList
        appointments={appointments}
        editingId={editingId}
        setEditingId={setEditingId}
        editForm={editForm}
        setEditForm={setEditForm}
        editOccupiedTimes={editOccupiedTimes}
        doctors={doctors}
        today={today}
        onUpdate={handleUpdate}
        onDelete={handleDelete}
      />

    </div>
  );
}

export default App;