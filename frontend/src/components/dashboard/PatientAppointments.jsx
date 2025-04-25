import React, { useState } from 'react';
import { Card, Button, Table, Modal, Form, Alert } from 'react-bootstrap';
import appointmentService from '../../services/appointmentService';

const PatientAppointments = ({ appointments, doctors, onAppointmentChange, user }) => {
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [showBookingModal, setShowBookingModal] = useState(false);
  const [selectedDoctor, setSelectedDoctor] = useState('');
  const [selectedDate, setSelectedDate] = useState('');
  const [selectedTimeSlot, setSelectedTimeSlot] = useState('');
  const [availableTimeSlots, setAvailableTimeSlots] = useState([]);

  const getDoctorName = (doctorId) => {
    const doctor = doctors.find(d => d.id === doctorId);
    return doctor ? `${doctor.firstName} ${doctor.lastName}` : `Doctor ${doctorId}`;
  };

  const handleDoctorChange = async (e) => {
    const doctorId = e.target.value;
    setSelectedDoctor(doctorId);
    if (doctorId && selectedDate) {
      await fetchAvailableTimeSlots(doctorId, selectedDate);
    }
  };

  const handleDateChange = async (e) => {
    const date = e.target.value;
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Set to start of day
    
    const selectedDate = new Date(date);
    selectedDate.setHours(0, 0, 0, 0);
    
    if (selectedDate < today) {
      setError('Please select a date from today onwards');
      setSelectedDate('');
      setAvailableTimeSlots([]);
      return;
    }
    
    setSelectedDate(date);
    setSelectedTimeSlot('');
    if (selectedDoctor) {
      await fetchAvailableTimeSlots(selectedDoctor, date);
    }
  };

  const fetchAvailableTimeSlots = async (doctorId, date) => {
    try {
      const slots = await appointmentService.getAvailableTimeSlots(doctorId, new Date(date));
      const sortedSlots = slots.sort((a, b) => {
        const timeA = new Date(`1970-01-01T${a.time}`);
        const timeB = new Date(`1970-01-01T${b.time}`);
        return timeA - timeB;
      });
      setAvailableTimeSlots(sortedSlots);
    } catch (error) {
      console.error('Error fetching time slots:', error);
      setError('Failed to fetch available time slots');
    }
  };

  const handleCancelAppointment = async (appointmentId) => {
    try {
      await appointmentService.cancelAppointment(appointmentId);
      setSuccess('Appointment cancelled successfully');
      onAppointmentChange();
    } catch (error) {
      console.error('Error cancelling appointment:', error);
      setError('Failed to cancel appointment');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!selectedDoctor || !selectedDate || !selectedTimeSlot) {
      setError('Please fill in all required fields');
      return;
    }

    try {
      const [hours, minutes] = selectedTimeSlot.split(':');
      const startTime = new Date(1970, 0, 1, parseInt(hours), parseInt(minutes), 0);
      const endTime = new Date(startTime.getTime() + 30 * 60 * 1000);

      const appointmentData = {
        doctorId: parseInt(selectedDoctor),
        patientId: user.id,
        appointmentDate: selectedDate,
        startTime: startTime.toTimeString().split(' ')[0],
        endTime: endTime.toTimeString().split(' ')[0],
        status: 'Scheduled'
      };

      await appointmentService.bookAppointment(appointmentData);
      setSuccess('Appointment booked successfully');
      setShowBookingModal(false);
      onAppointmentChange();
    } catch (error) {
      console.error('Error booking appointment:', error);
      setError('Failed to book appointment');
    }
  };

  return (
    <>
      {error && <Alert variant="danger">{error}</Alert>}
      {success && <Alert variant="success">{success}</Alert>}

      <Card className="mb-4">
        <Card.Header>
          <div className="d-flex justify-content-between align-items-center">
            <h4>My Appointments</h4>
            <Button variant="primary" onClick={() => setShowBookingModal(true)}>
              Book New Appointment
            </Button>
          </div>
        </Card.Header>
        <Card.Body>
          <Table striped hover>
            <thead>
              <tr>
                <th>Date</th>
                <th>Time</th>
                <th>Doctor</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {appointments.map((appointment) => (
                <tr key={appointment.id}>
                  <td>{new Date(appointment.appointmentDate).toLocaleDateString()}</td>
                  <td>
                    {new Date(`1970-01-01T${appointment.startTime}`).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })} - 
                    {new Date(`1970-01-01T${appointment.endTime}`).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                  </td>
                  <td>{getDoctorName(appointment.doctorId)}</td>
                  <td>
                    <span className={`badge bg-${appointment.status === 'Scheduled' ? 'success' : 'warning'}`}>
                      {appointment.status}
                    </span>
                  </td>
                  <td>
                    {appointment.status === 'Scheduled' && (
                      <Button 
                        variant="outline-danger" 
                        size="sm"
                        onClick={() => handleCancelAppointment(appointment.id)}
                      >
                        Cancel
                      </Button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </Table>
        </Card.Body>
      </Card>

      <Modal show={showBookingModal} onHide={() => setShowBookingModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Book New Appointment</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Select Doctor</Form.Label>
              <Form.Select
                value={selectedDoctor}
                onChange={handleDoctorChange}
                required
              >
                <option value="">Choose a doctor</option>
                {doctors.map((doctor) => (
                  <option key={doctor.id} value={doctor.id}>
                    {`${doctor.firstName} ${doctor.lastName}`}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Select Date</Form.Label>
              <Form.Control
                type="date"
                value={selectedDate}
                onChange={handleDateChange}
                min={new Date().toISOString().split('T')[0]}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Select Time Slot</Form.Label>
              <Form.Select
                value={selectedTimeSlot}
                onChange={(e) => setSelectedTimeSlot(e.target.value)}
                required
                disabled={!selectedDoctor || !selectedDate}
              >
                <option value="">Choose a time slot</option>
                {availableTimeSlots
                  .filter(slot => slot.isAvailable)
                  .map((slot, index) => (
                    <option key={index} value={slot.time}>
                      {new Date(`1970-01-01T${slot.time}`).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                    </option>
                  ))}
              </Form.Select>
            </Form.Group>

            <Button variant="primary" type="submit">
              Book Appointment
            </Button>
          </Form>
        </Modal.Body>
      </Modal>
    </>
  );
};

export default PatientAppointments; 