import React, { useState } from 'react';
import { Card, Button, Form, Row, Col } from 'react-bootstrap';

const PatientProfile = ({ patient, user, onUpdate, setPatient }) => {
  const [isEditing, setIsEditing] = useState(false);
  const [editedProfile, setEditedProfile] = useState({
    name: patient?.name || '',
    phone: user?.phone || '',
    birthDate: patient?.birthDate ? new Date(patient.birthDate).toISOString().split('T')[0] : '',
    address: user?.address || '',
    medicalHistory: patient?.medicalHistory || ''
  });

  const handleSave = async () => {
    try {
      const updatedProfile = await onUpdate(editedProfile);
      setPatient(updatedProfile);
      setIsEditing(false);
    } catch (error) {
      console.error('Error updating profile:', error);
    }
  };

  const handleCancel = () => {
    setEditedProfile({
      name: patient?.name || '',
      phone: patient?.user?.phone || '',
      birthDate: patient?.birthDate ? new Date(patient.birthDate).toISOString().split('T')[0] : '',
      address: patient?.user?.address || '',
      medicalHistory: patient?.medicalHistory || ''
    });
    setIsEditing(false);
  };

  return (
    <Card className="mb-4">
      <Card.Header className="d-flex justify-content-between align-items-center">
        <h5>My Profile</h5>
        {!isEditing ? (
          <Button variant="outline-primary" onClick={() => setIsEditing(true)}>
            Edit Profile
          </Button>
        ) : (
          <div>
            <Button variant="primary" onClick={handleSave} className="me-2">
              Save
            </Button>
            <Button variant="outline-secondary" onClick={handleCancel}>
              Cancel
            </Button>
          </div>
        )}
      </Card.Header>
      <Card.Body>
        {isEditing ? (
          <Form>
            <Row>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>Name</Form.Label>
                  <Form.Control
                    type="text"
                    value={editedProfile.name}
                    onChange={(e) => setEditedProfile({...editedProfile, name: e.target.value})}
                  />
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label>Phone</Form.Label>
                  <Form.Control
                    type="text"
                    value={editedProfile.phone}
                    onChange={(e) => setEditedProfile({...editedProfile, phone: e.target.value})}
                  />
                </Form.Group>
              </Col>
              <Col md={6}>
                <Form.Group className="mb-3">
                  <Form.Label>Date of Birth</Form.Label>
                  <Form.Control
                    type="date"
                    value={editedProfile.birthDate}
                    onChange={(e) => setEditedProfile({...editedProfile, birthDate: e.target.value})}
                  />
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label>Address</Form.Label>
                  <Form.Control
                    type="text"
                    value={editedProfile.address}
                    onChange={(e) => setEditedProfile({...editedProfile, address: e.target.value})}
                  />
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label>medicalHistory</Form.Label>
                  <Form.Control
                    type="text"
                    value={editedProfile.medicalHistory}
                    onChange={(e) => setEditedProfile({...editedProfile, medicalHistory: e.target.value})}
                  />
                </Form.Group>
              </Col>
            </Row>
          </Form>
        ) : (
          <Row>
            <Col md={6}>
              <p><strong>Name:</strong> {patient?.name}</p>
              <p><strong>Email:</strong> {user?.email}</p>
              <p><strong>Phone:</strong> {patient?.user?.phone || 'Not provided'}</p>
            </Col>
            <Col md={6}>
              <p><strong>Date of Birth:</strong> {patient?.birthDate ? new Date(patient.birthDate).toLocaleDateString() : 'Not provided'}</p>
              <p><strong>Address:</strong> {patient?.user?.address || 'Not provided'}</p>
              <p><strong>MedicalHistory:</strong> {patient?.medicalHistory || 'Not provided'}</p>
            </Col>
          </Row>
        )}
      </Card.Body>
    </Card>
  );
};

export default PatientProfile; 