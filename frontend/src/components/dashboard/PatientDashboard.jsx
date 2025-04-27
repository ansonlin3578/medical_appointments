import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Alert, Button } from 'react-bootstrap';
import { useAuth } from '../../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import PatientProfile from './PatientProfile';
import PatientAppointments from './PatientAppointments';
import doctorService from '../../services/doctorService';
import appointmentService from '../../services/appointmentService';
import patientService from '../../services/patientService';

const PatientDashboard = ({ user }) => {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState('');
  const [doctors, setDoctors] = useState([]);
  const [appointments, setAppointments] = useState([]);
  const [patient, setPatient] = useState(null);

  useEffect(() => {
    fetchDoctors();
    fetchAppointments();
    fetchPatientProfile();
  }, []);

  const fetchDoctors = async () => {
    try {
      const data = await doctorService.getDoctors();
      setDoctors(data);
    } catch (error) {
      console.error('Error fetching doctors:', error);
      setError('Failed to fetch doctors');
    }
  };

  const fetchAppointments = async () => {
    try {
      const data = await appointmentService.getPatientAppointments(user.id);
      setAppointments(data);
    } catch (error) {
      console.error('Error fetching appointments:', error);
      setError('Failed to fetch appointments');
    }
  };

  const fetchPatientProfile = async () => {
    try {
      const data = await patientService.getPatientProfile(user.id);
      setPatient(data);
    } catch (error) {
      console.error('Error fetching patient profile:', error);
      setError('Failed to fetch patient profile');
    }
  };

  const handleProfileUpdate = async (profileData) => {
    try {
      const updatedProfile = await patientService.updatePatientProfile(user.id, profileData);
      setPatient(updatedProfile);
      return updatedProfile;
    } catch (error) {
      console.error('Error updating profile:', error);
      setError('Failed to update profile');
      throw error;
    }
  };

  const handleAppointmentChange = () => {
    fetchAppointments();
  };

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Container className="py-4">
      {error && <Alert variant="danger">{error}</Alert>}
      
      <Row className="mb-4">
        <Col>
          <h2>Patient Dashboard</h2>
          <p className="text-muted">Welcome, {user.username}</p>
        </Col>
        <Col xs="auto">
          <Button variant="outline-danger" onClick={handleLogout}>
            Logout
          </Button>
        </Col>
      </Row>

      <Row className="mb-4">
        <Col>
          <PatientProfile 
            patient={patient} 
            user={user} 
            onUpdate={handleProfileUpdate}
            setPatient={setPatient}
          />
        </Col>
      </Row>

      <Row>
        <Col>
          <PatientAppointments 
            appointments={appointments}
            doctors={doctors}
            onAppointmentChange={handleAppointmentChange}
            user={user}
          />
        </Col>
      </Row>
    </Container>
  );
};

export default PatientDashboard; 