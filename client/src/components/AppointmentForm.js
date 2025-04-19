// components/AppointmentForm.js
import React from 'react';
import TimeSlotSelect from './TimeSlotSelect';

function AppointmentForm({ form, setForm, doctors, occupiedTimes, onSubmit, today }) {
  return (
    <form onSubmit={onSubmit}>
      <input
        placeholder="姓名"
        value={form.name}
        onChange={(e) => setForm({ ...form, name: e.target.value })}
      />
      <input
        type="date"
        min={today}
        value={form.date}
        onChange={(e) => setForm({ ...form, date: e.target.value })}
      />
      <select
        value={form.doctor_id}
        onChange={(e) => setForm({ ...form, doctor_id: e.target.value })}
      >
        <option value="">請選擇醫生</option>
        {doctors.map((doc) => (
          <option key={doc.id} value={doc.id}>{doc.name}</option>
        ))}
      </select>

      <TimeSlotSelect
        value={form.start_time}
        onChange={(start, end) => setForm({ ...form, start_time: start, end_time: end })}
        occupiedTimes={occupiedTimes}
      />

      <button type="submit">預約</button>
    </form>
  );
}

export default AppointmentForm;
