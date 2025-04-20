import React from 'react';

function DoctorSelect({ name, value, onChange, doctors }) {
  return (
    <select name={name} value={value} onChange={onChange}>
      {doctors.map((doc) => (
        <option key={doc.id} value={doc.id}>
          {doc.name}
        </option>
      ))}
    </select>
  );
}

export default DoctorSelect;
