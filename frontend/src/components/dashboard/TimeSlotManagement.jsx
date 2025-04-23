import React, { useState, useEffect } from 'react';
import { Form, Button, Table, Modal } from 'react-bootstrap';
import { format, parseISO } from 'date-fns';
import timeSlotService from '../../api/timeSlotService.jsx';

const TimeSlotManagement = ({ doctorId, onError, onSuccess }) => {
  const [timeSlots, setTimeSlots] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [dayOfWeek, setDayOfWeek] = useState(1); // Monday = 1
  const [startTime, setStartTime] = useState('09:00');
  const [endTime, setEndTime] = useState('10:00');
  const [notes, setNotes] = useState('');
  const [loading, setLoading] = useState(false);

  // Generate time slots from 9:00 to 18:00 with 1-hour intervals
  const timeOptions = Array.from({ length: 10 }, (_, i) => {
    const hour = i + 9;
    return `${hour.toString().padStart(2, '0')}:00`;
  });

  const daysOfWeek = [
    { value: 0, label: 'Sunday' },
    { value: 1, label: 'Monday' },
    { value: 2, label: 'Tuesday' },
    { value: 3, label: 'Wednesday' },
    { value: 4, label: 'Thursday' },
    { value: 5, label: 'Friday' },
    { value: 6, label: 'Saturday' }
  ];

  useEffect(() => {
    fetchTimeSlots();
  }, [doctorId]);

  const fetchTimeSlots = async () => {
    try {
      const slots = await timeSlotService.getDoctorSchedules(doctorId);
      setTimeSlots(slots);
    } catch (error) {
      onError('Failed to fetch time slots');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const schedule = {
        doctorId,
        dayOfWeek,
        startTime: `${startTime}:00`, // Add seconds to match TimeSpan format
        endTime: `${endTime}:00`,     // Add seconds to match TimeSpan format
        isAvailable: true,
        notes: notes || '' // Ensure notes is always a string
      };

      await timeSlotService.createTimeSlot(schedule);
      await fetchTimeSlots();
      setShowModal(false);
      onSuccess('Time slot added successfully');
      resetForm();
    } catch (error) {
      onError('Failed to add time slot');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (slotId) => {
    if (window.confirm('Are you sure you want to delete this time slot?')) {
      try {
        await timeSlotService.deleteTimeSlot(slotId);
        await fetchTimeSlots();
        onSuccess('Time slot deleted successfully');
      } catch (error) {
        onError('Failed to delete time slot');
      }
    }
  };

  const resetForm = () => {
    setDayOfWeek(1); // Reset to Monday
    setStartTime('09:00');
    setEndTime('10:00');
    setNotes('');
  };

  return (
    <div>
      <Button variant="primary" onClick={() => setShowModal(true)} className="mb-3">
        Add New Time Slot
      </Button>

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>Day</th>
            <th>Start Time</th>
            <th>End Time</th>
            <th>Notes</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {timeSlots.map((slot) => (
            <tr key={slot.id}>
              <td>{daysOfWeek.find(d => d.value === slot.dayOfWeek)?.label || 'Unknown'}</td>
              <td>{slot.startTime}</td>
              <td>{slot.endTime}</td>
              <td>{slot.isAvailable ? 'Available' : 'Unavailable'}</td>
              <td>{slot.notes}</td>
              <td>
                <Button
                  variant="danger"
                  size="sm"
                  onClick={() => handleDelete(slot.id)}
                >
                  Delete
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Add New Time Slot</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Day of Week</Form.Label>
              <Form.Select
                value={dayOfWeek}
                onChange={(e) => setDayOfWeek(parseInt(e.target.value))}
                required
              >
                {daysOfWeek.map((day) => (
                  <option key={day.value} value={day.value}>
                    {day.label}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Start Time</Form.Label>
              <Form.Select
                value={startTime}
                onChange={(e) => setStartTime(e.target.value)}
                required
              >
                {timeOptions.map((time) => (
                  <option key={time} value={time}>
                    {time}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>End Time</Form.Label>
              <Form.Select
                value={endTime}
                onChange={(e) => setEndTime(e.target.value)}
                required
              >
                {timeOptions.map((time) => (
                  <option key={time} value={time}>
                    {time}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Notes</Form.Label>
              <Form.Control
                as="textarea"
                rows={3}
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
                placeholder="Optional notes about this time slot"
              />
            </Form.Group>

            <Button variant="primary" type="submit" disabled={loading}>
              {loading ? 'Adding...' : 'Add Time Slot'}
            </Button>
          </Form>
        </Modal.Body>
      </Modal>
    </div>
  );
};

export default TimeSlotManagement; 