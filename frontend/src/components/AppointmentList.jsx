import React from 'react';
import { generateHourlySlots, calculateEndTime } from '../utils/timeSlots';
import { formatTime } from '../utils/formatTime';

const availableHours = generateHourlySlots();

function AppointmentList({
  appointments,
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
  const handleEditChange = (e) => {
    setEditForm({ ...editForm, [e.target.name]: e.target.value });
  };

  const handleEditTimeChange = (e) => {
    const start = e.target.value;
    const end = calculateEndTime(start);
    setEditForm({ ...editForm, startTime: start, endTime: end });
  };  

  return (
    <ul>
      {appointments.length === 0 ? (
        <li>目前沒有預約</li>
      ) : (
        appointments.map((a) => {
          const isEditing = editingId === a.id;
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

                  <select
                    name="doctorId"
                    value={editForm.doctorId}
                    onChange={handleEditChange}
                  >
                    {doctors.map((doc) => (
                      <option key={doc.id} value={doc.id}>
                        {doc.name}
                      </option>
                    ))}
                  </select>


                  <select name="startTime" value={editForm.startTime.slice(0, 8)} onChange={handleEditTimeChange}>
                    {availableHours.map(({ start, label }) => (
                        <option key={start} value={start} disabled={editOccupiedTimes.includes(start)}>
                        {label}
                        </option>
                    ))}
                </select>

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
                  <button onClick={() => {
                    setEditingId(a.id);
                    setEditForm({
                      name: a.name,
                      date: a.date,
                      startTime: a.startTime,
                      endTime: a.endTime,
                      doctorId: a.doctorId
                    });
                  }}>編輯</button>
                  <button onClick={() => onDelete(a.id)}>刪除</button>
                </>
              )}
            </li>
          );
        })
      )}
    </ul>
  );
}

export default AppointmentList;
