// components/TimeSlotSelect.js
function TimeSlotSelect({ value, onChange, occupiedTimes }) {
    const hours = [...Array(9)].map((_, i) => 9 + i);
  
    return (
      <select
        value={value}
        onChange={(e) => {
          const [h, m] = e.target.value.split(':').map(Number);
          const end = `${String(h + 1).padStart(2, '0')}:${String(m).padStart(2, '0')}`;
          onChange(e.target.value, end);
        }}
      >
        <option value="">請選擇時段</option>
        {hours.map((h) => {
          const start = `${String(h).padStart(2, '0')}:00`;
          const end = `${String(h + 1).padStart(2, '0')}:00`;
          const isOccupied = occupiedTimes.includes(start);
          return (
            <option key={start} value={start} disabled={isOccupied}>
              {start} - {end} {isOccupied ? '(已預約)' : ''}
            </option>
          );
        })}
      </select>
    );
  }
  
  export default TimeSlotSelect;
  