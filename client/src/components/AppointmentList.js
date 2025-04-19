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
  if (appointments.length === 0) {
    return <p>目前沒有預約</p>;
  }

  return (
    <ul>
      {appointments.map((a) => (
        <AppointmentItem
          key={a.id}
          appointment={a}
          editingId={editingId}
          setEditingId={setEditingId}
          editForm={editForm}
          setEditForm={setEditForm}
          editOccupiedTimes={editOccupiedTimes}
          doctors={doctors}
          today={today}
          onUpdate={onUpdate}
          onDelete={onDelete}
        />
      ))}
    </ul>
  );
}

export default AppointmentList;
