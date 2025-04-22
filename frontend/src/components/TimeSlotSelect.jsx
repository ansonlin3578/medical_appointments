import React from 'react';

function TimeSlotSelect({ name, value, onChange, availableHours, occupiedTimes }) {
  return (
    <select name={name} value={value} onChange={onChange}>
      {availableHours.map(({ start, label }) => (
        <option key={start} value={start} disabled={occupiedTimes.includes(start)}>
          {label}
        </option>
      ))}
    </select>
  );
}

export default TimeSlotSelect;
