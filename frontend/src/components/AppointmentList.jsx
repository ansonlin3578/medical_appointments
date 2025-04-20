import React from 'react';
import AppointmentItem from './AppointmentItem';

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
  return (
        <ul>
    {appointments.length === 0 ? (
        <li>目前沒有預約</li>
    ) : (
        appointments.map((a) => (
        <AppointmentItem
            key={a.id}
            a={a}
            isEditing={editingId === a.id}
            today={today}
            doctors={doctors}
            editForm={editForm}
            setEditForm={setEditForm}
            editOccupiedTimes={editOccupiedTimes}
            onUpdate={onUpdate}
            onDelete={onDelete}
            setEditingId={setEditingId}
        />
        ))
    )}
    </ul>
  );
}

export default AppointmentList;
