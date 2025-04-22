// src/utils/timeSlots.js

// 回傳每小時區間物件陣列
export const generateHourlySlots = (startHour = 9, endHour = 18) => {
    const slots = [];
    for (let hour = startHour; hour < endHour; hour++) {
      const start = `${String(hour).padStart(2, '0')}:00:00`;
      const end = `${String(hour + 1).padStart(2, '0')}:00:00`;
      const label = `${start.slice(0, 5)} - ${end.slice(0, 5)}`;
      slots.push({ start, end, label });
    }
    return slots;
  };
  
  // 傳入 "09:00:00" 會回傳 "10:00:00"
  export const calculateEndTime = (startTime) => {
    const [hour, min] = startTime.split(':').map(Number);
    const endHour = hour + 1;
    return `${String(endHour).padStart(2, '0')}:${String(min).padStart(2, '0')}:00`;
  };
  