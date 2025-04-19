import React from 'react';
import TimeSlotSelect from './TimeSlotSelect';

function AppointmentItem({
  appointment,
  editingId,
  setEditingId,
  editForm,
  setEditForm,
  editOccupiedTimes,
  doctors,
  today,
  onUpdate,
  onDelete
}) {
  const a = appointment;
  const isEditing = editingId === a.id;

  if (isEditing) {
    return (
      <li>
        <input
          type="date"
          min={today}
          value={editForm.date}
          onChange={(e) => setEditForm({ ...editForm, date: e.target.value })}
        />
        <select
          value={editForm.doctor_id}
          onChange={(e) => setEditForm({ ...editForm, doctor_id: e.target.value })}
        >
          {doctors.map((doc) => (
            <option key={doc.id} value={doc.id}>{doc.name}</option>
          ))}
        </select>
        <TimeSlotSelect
          value={editForm.start_time}
          occupiedTimes={editOccupiedTimes}
          onChange={(start, end) => setEditForm({ ...editForm, start_time: start, end_time: end })}
        />
        <button onClick={() => onUpdate(a.id)}>儲存</button>
        <button onClick={() => setEditingId(null)}>取消</button>
      </li>
    );
  }

  return (
    <li>
      {a.name} - 日期: {a.date} - 時段: {a.start_time} ~ {a.end_time} - 醫生: {a.doctor_name}
      <button onClick={() => {
        console.log("content: ", a)
        setEditForm({
          date: a.date,
          start_time: a.start_time,
          end_time: a.end_time,
          doctor_id: a.doctor_id
        });
        setEditingId(a.id);
      }} style={{ marginLeft: 10 }}>修改</button>
      <button onClick={() => onDelete(a.id)} style={{ marginLeft: 10 }}>刪除</button>
    </li>
  );
}

export default AppointmentItem;
