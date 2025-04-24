import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Button, Table, Modal, Form, Alert } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import appointmentService from '../../services/appointmentService';
import Roles from '../../constants/roles';

const PatientDashboard = () => {
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [appointments, setAppointments] = useState([]);
  const [doctors, setDoctors] = useState([]);
  const [availableTimeSlots, setAvailableTimeSlots] = useState([]);
  const [showBookingModal, setShowBookingModal] = useState(false);
  const [selectedDoctor, setSelectedDoctor] = useState('');
  const [selectedDate, setSelectedDate] = useState('');
  const [selectedTimeSlot, setSelectedTimeSlot] = useState('');
  const [newAppointment, setNewAppointment] = useState({
    patientId: '',
    doctorId: '',
    date: '',
    timeSlotId: ''
  });

  useEffect(() => {
    console.log('PatientDashboard useEffect triggered', { 
      user, 
      hasUser: !!user, 
      userRole: user?.role,
      isPatient: user?.role?.toLowerCase() === Roles.PATIENT.toLowerCase()
    });
    if (user && user.role?.toLowerCase() === Roles.PATIENT.toLowerCase()) {
      fetchAppointments();
      fetchDoctors();
    } else {
      console.log('Not fetching data because:', {
        noUser: !user,
        wrongRole: user?.role?.toLowerCase() !== Roles.PATIENT.toLowerCase()
      });
    }
  }, [user]);

  useEffect(() => {
    if (selectedDoctor && selectedDate) {
      fetchAvailableTimeSlots();
    } else {
      setAvailableTimeSlots([]);
    }
  }, [selectedDoctor, selectedDate]);

  const fetchAppointments = async () => {
    try {
      console.log('Fetching appointments...');
      const data = await appointmentService.getPatientAppointments();
      console.log('Received appointments:', data);
      setAppointments(data);
    } catch (err) {
      console.error('Error fetching appointments:', err);
      setError('Failed to fetch appointments');
    }
  };

  const fetchDoctors = async () => {
    try {
      const data = await appointmentService.getAvailableDoctors();
      console.log('Doctors data:', data);
      setDoctors(data);
    } catch (err) {
      console.error('Error in fetchDoctors:', err);
      setError('Failed to fetch doctors');
    }
  };

  const fetchAvailableTimeSlots = async () => {
    try {
      console.log('Fetching time slots for:', {
        doctorId: selectedDoctor,
        date: selectedDate
      });

      const slots = await appointmentService.getAvailableTimeSlots(selectedDoctor, new Date(selectedDate));
      console.log('Available time slots:', slots);

      // Sort time slots by time
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

  const handleDoctorChange = (e) => {
    const doctorId = e.target.value;
    setSelectedDoctor(doctorId);
    if (doctorId && selectedDate) {
      fetchAvailableTimeSlots();
    }
  };

  const handleDateChange = (e) => {
    const selectedDate = e.target.value;
    const today = new Date().toISOString().split('T')[0];
    
    if (selectedDate < today) {
      setError('Please select a future date');
      return;
    }
    
    setSelectedDate(selectedDate);
    setSelectedTimeSlot('');
  };

  const handleTimeSlotChange = (e) => {
    setSelectedTimeSlot(e.target.value);
  };

  const handleBookAppointment = async (e) => {
    e.preventDefault();
    try {
      const appointmentData = {
        patientId: user.id,
        doctorId: selectedDoctor,
        appointmentDate: selectedDate,
        timeSlot: selectedTimeSlot
      };
      await appointmentService.bookAppointment(appointmentData);
      setSuccess('Appointment booked successfully');
      setShowBookingModal(false);
      fetchAppointments();
    } catch (err) {
      setError('Failed to book appointment');
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const handleClose = () => {
    setShowBookingModal(false);
    setNewAppointment({
      patientId: '',
      doctorId: '',
      date: '',
      timeSlotId: ''
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!selectedDoctor || !selectedDate || !selectedTimeSlot) {
      setError('Please fill in all required fields');
      return;
    }

    try {
      // Parse the time slot into hours, minutes, seconds
      const [hours, minutes] = selectedTimeSlot.split(':');
      const startTime = new Date(1970, 0, 1, parseInt(hours), parseInt(minutes), 0);
      const endTime = new Date(startTime.getTime() + 30 * 60 * 1000); // Add 30 minutes

      const appointmentData = {
        doctorId: parseInt(selectedDoctor),
        patientId: user.id,
        appointmentDate: selectedDate,
        startTime: startTime.toTimeString().split(' ')[0], // Format as HH:mm:ss
        endTime: endTime.toTimeString().split(' ')[0],     // Format as HH:mm:ss
        status: 'Scheduled'
      };

      console.log('Sending appointment data:', appointmentData);
      await appointmentService.bookAppointment(appointmentData);
      setSuccess('Appointment booked successfully');
      setShowBookingModal(false);
      fetchAppointments(); // Refresh appointments list
    } catch (error) {
      console.error('Error booking appointment:', error);
      setError('Failed to book appointment');
    }
  };

  const getDoctorName = (doctorId) => {
    const doctor = doctors.find(d => d.id === doctorId);
    return doctor ? `${doctor.firstName} ${doctor.lastName}` : `Doctor ${doctorId}`;
  };

  return (
    <Container className="mt-4">
      <Row className="mb-4">
        <Col>
          <h2>Patient Dashboard</h2>
        </Col>
        <Col className="text-end">
          <Button variant="outline-danger" onClick={handleLogout}>
            Logout
          </Button>
        </Col>
      </Row>

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
                </tr>
              ))}
            </tbody>
          </Table>
        </Card.Body>
      </Card>

      <Modal show={showBookingModal} onHide={handleClose}>
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
              <Form.Text className="text-muted">
                Please select a future date for your appointment
              </Form.Text>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Select Time Slot</Form.Label>
              <Form.Select
                value={selectedTimeSlot}
                onChange={handleTimeSlotChange}
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
    </Container>
  );
};

export default PatientDashboard; 