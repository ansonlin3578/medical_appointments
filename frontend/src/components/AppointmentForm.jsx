import React from 'react';
import { generateHourlySlots, calculateEndTime } from '../utils/timeSlots';

const availableHours = generateHourlySlots();

function AppointmentForm({ form, setForm, doctors, occupiedTimes, onSubmit, today }) {
  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleTimeChange = (e) => {
    const start = e.target.value;
    const end = calculateEndTime(start);
    setForm({ ...form, startTime: start, endTime: end });
  };

  return (
    <form onSubmit={onSubmit} style={{ marginBottom: 40 }}>
      <input
        type="text"
        name="name"
        placeholder="姓名"
        value={form.name}
        onChange={handleChange}
      />

      <input
        type="date"
        name="date"
        min={today}
        value={form.date}
        onChange={handleChange}
      />

      <select name="doctorId" value={form.doctorId} onChange={handleChange}>
        <option value="">請選擇醫師</option>
        {doctors.map((doc) => (
          <option key={doc.id} value={doc.id}>
            {doc.name}
          </option>
        ))}
      </select>

      <select name="startTime" value={form.startTime} onChange={handleTimeChange}>
        <option value="">請選擇時段</option>
        {availableHours.map(({ start, label }) => (
          <option key={start} value={start} disabled={occupiedTimes.includes(start)}>
            {label}
          </option>
        ))}
      </select>

      <button type="submit">送出預約</button>
    </form>
  );
}

export default AppointmentForm;
