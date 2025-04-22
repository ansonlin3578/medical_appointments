import React from 'react';
import { generateHourlySlots, calculateEndTime } from '../utils/timeSlots';
import { formatTime } from '../utils/formatTime';
import DoctorSelect from './DoctorSelect';
import TimeSlotSelect from './TimeSlotSelect';

const availableHours = generateHourlySlots();

function AppointmentItem({
  a,
  isEditing,
  today,
  doctors,
  editForm,
  setEditForm,
  editOccupiedTimes,
  onUpdate,
  onDelete,
  setEditingId
}) {
  const handleEditChange = (e) => {
    setEditForm({ ...editForm, [e.target.name]: e.target.value });
  };

  const handleEditTimeChange = (e) => {
    const start = e.target.value;
    const end = calculateEndTime(start);
    setEditForm({ ...editForm, startTime: start, endTime: end });
  };

  const doctorName = doctors.find((d) => d.id === a.doctorId)?.name || '未知';

  return (
    <li key={a.id} style={{ marginBottom: 20 }}>
      {isEditing ? (
        <>
          <input
            type="date"
            name="date"
            min={today}
            value={editForm.date}
            onChange={handleEditChange}
          />

          <DoctorSelect
            name="doctorId"
            value={editForm.doctorId}
            onChange={handleEditChange}
            doctors={doctors}
          />

          <TimeSlotSelect
            name="startTime"
            value={editForm.startTime.slice(0, 8)}
            onChange={handleEditTimeChange}
            availableHours={availableHours}
            occupiedTimes={editOccupiedTimes}
          />

          <input
            type="text"
            name="name"
            value={editForm.name || ''}
            onChange={handleEditChange}
          />

          <button onClick={() => onUpdate(a.id)}>儲存</button>
          <button onClick={() => setEditingId(null)}>取消</button>
        </>
      ) : (
        <>
          <span>
            👤 {a.name} 🧑‍⚕️ {doctorName} 📅 {a.date} 🕐 {formatTime(a.startTime)} ~ {formatTime(a.endTime)}
          </span>
          <button
            onClick={() => {
              setEditingId(a.id);
              setEditForm({
                name: a.name,
                date: a.date,
                startTime: a.startTime,
                endTime: a.endTime,
                doctorId: a.doctorId
              });
            }}
          >
            編輯
          </button>
          <button onClick={() => onDelete(a.id)}>刪除</button>
        </>
      )}
    </li>
  );
}

export default AppointmentItem;
