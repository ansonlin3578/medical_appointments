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
  const [form, setForm] = useState({ name: '', date: '', startTime: '', endTime: '', doctorId: '' });
  const [doctors, setDoctors] = useState([]);
  const today = new Date().toISOString().split('T')[0];
  const [occupiedTimes, setOccupiedTimes] = useState([]);
  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({ date: '', startTime: '', endTime: '', doctorId: '' });
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
    console.log(form);
    if (!form.name || !form.date || !form.startTime || !form.endTime || !form.doctorId) {
      alert('請輸入完整資料（姓名、日期、時段、醫生）');
      return;
    }
  
    try {
      await addAppointment(form);
      setForm({ name: '', date: '', startTime: '', endTime: '', doctorId: '' });
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
      if (!form.date || !form.doctorId) {
        setOccupiedTimes([]);
        return;
      }
    
      try {
        const times = await getOccupiedTimes(form.date, form.doctorId);
        setOccupiedTimes(times);
      } catch (err) {
        console.error('Error fetching occupied times:', err);
      }
    };
  
    fetchOccupiedTimes();
  }, [form.date, form.doctorId]);

  useEffect(() => {
    const fetchEditOccupied = async () => {
      if (!editingId || !editForm.date || !editForm.doctorId) {
        setEditOccupiedTimes([]);
        return;
      }
    
      try {
        const times = await getOccupiedTimes(editForm.date, editForm.doctorId);
        setEditOccupiedTimes(times.filter(t => t !== editForm.startTime));
      } catch (err) {
        console.error('Error fetching edit occupied times:', err);
      }
    };
  
    fetchEditOccupied();
  }, [editingId, editForm.date, editForm.doctorId, editForm.startTime]);
  

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