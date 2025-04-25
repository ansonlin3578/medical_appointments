import React, { useState, useEffect } from 'react';
import { useAuth } from '../../context/AuthContext';
import { Container, Row, Col, Card, Button, Alert } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import TimeSlotManagement from './TimeSlotManagement';
import DoctorProfile from './DoctorProfile';

const DoctorDashboard = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [currentUser, setCurrentUser] = useState(user);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const handleUserUpdate = (updatedUser) => {
    setCurrentUser(updatedUser);
  };

  if (!currentUser || currentUser.role !== 'Doctor') {
    return (
      <Container className="mt-5">
        <Alert variant="danger">Access denied. This page is for doctors only.</Alert>
      </Container>
    );
  }

  return (
    <Container className="py-4">
      {error && <Alert variant="danger">{error}</Alert>}
      
      <Row className="mb-4">
        <Col>
          <h2>Doctor Dashboard</h2>
          <p className="text-muted">Welcome, {currentUser.username}</p>
        </Col>
        <Col xs="auto">
          <Button variant="outline-danger" onClick={handleLogout}>
            Logout
          </Button>
        </Col>
      </Row>

      <Row className="mb-4">
        <Col>
          <DoctorProfile 
            user={currentUser}
            onUserUpdate={handleUserUpdate}
          />
        </Col>
      </Row>

      <Row className="justify-content-between align-items-center">
        <Col>
          <h4>Manage Your Available Time Slots</h4>
        </Col>
      </Row>
      
      {success && (
        <Row className="mt-3">
          <Col>
            <Alert variant="success" onClose={() => setSuccess(null)} dismissible>
              {success}
            </Alert>
          </Col>
        </Row>
      )}

      <Row className="mt-4">
        <Col>
          <Card>
            <Card.Header>
              <h4>Manage Your Available Time Slots</h4>
            </Card.Header>
            <Card.Body>
              <TimeSlotManagement 
                doctorId={currentUser.id}
                onError={setError}
                onSuccess={setSuccess}
              />
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default DoctorDashboard; 