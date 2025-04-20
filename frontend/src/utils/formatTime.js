// src/utils/formatTime.js
export const formatTime = (time) => {
    if (!time) return '';
    return time.slice(0, 5); // e.g., "09:00:00" => "09:00"
  };
  