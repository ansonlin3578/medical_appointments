import React, { useState, useEffect } from 'react';
import { Card, Button, Form, Row, Col, ListGroup } from 'react-bootstrap';
import doctorService from '../../services/doctorService';

const DoctorProfile = ({ user: initialUser, onUserUpdate }) => {
  const [specialties, setSpecialties] = useState([]);
  const [editingSpecialty, setEditingSpecialty] = useState(null);
  const [isEditingProfile, setIsEditingProfile] = useState(false);
  const [user, setUser] = useState(initialUser);
  const [profileData, setProfileData] = useState({
    firstName: initialUser?.firstName || '',
    lastName: initialUser?.lastName || '',
    email: initialUser?.email || '',
    phone: initialUser?.phone || '',
    address: initialUser?.address || ''
  });

  useEffect(() => {
    if (user?.id) {
      fetchProfile();
      fetchSpecialties();
    }
  }, [user?.id]);

  const fetchProfile = async () => {
    try {
      const updatedUser = await doctorService.getDoctorProfile(user.id);
      setUser(updatedUser);
      setProfileData({
        firstName: updatedUser.firstName || '',
        lastName: updatedUser.lastName || '',
        email: updatedUser.email || '',
        phone: updatedUser.phone || '',
        address: updatedUser.address || ''
      });
      if (onUserUpdate) {
        onUserUpdate(updatedUser);
      }
    } catch (error) {
      console.error('Error fetching profile:', error);
    }
  };

  const handleProfileChange = (e) => {
    const { name, value } = e.target;
    setProfileData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSaveProfile = async () => {
    try {
      await doctorService.updateDoctorProfile(user.id, profileData);
      await fetchProfile();
      setIsEditingProfile(false);
    } catch (error) {
      console.error('Error updating profile:', error);
    }
  };

  const fetchSpecialties = async () => {
    try {
      const response = await doctorService.getDoctorSpecialties(user.id);
        setSpecialties(response);
    } catch (error) {
      console.error('Error fetching specialties:', error);
    }
  };

  const handleEditSpecialty = (specialty) => {
    setEditingSpecialty(specialty);
  };

  const handleSaveSpecialty = async () => {
    try {
      // Create a properly structured specialty object
      const specialtyToUpdate = {
        id: editingSpecialty.id,
        doctorId: editingSpecialty.doctorId,
        specialty: editingSpecialty.specialty,
        description: editingSpecialty.description,
        yearsOfExperience: editingSpecialty.yearsOfExperience
      };
      
      console.log('Sending specialty update:', specialtyToUpdate);
      await doctorService.updateSpecialty(editingSpecialty.id, specialtyToUpdate);
      setEditingSpecialty(null);
      fetchSpecialties();
    } catch (error) {
      console.error('Error updating specialty:', error);
    }
  };

  const handleDeleteSpecialty = async (specialtyId) => {
    try {
      await doctorService.removeSpecialty(specialtyId);
      fetchSpecialties();
    } catch (error) {
      console.error('Error deleting specialty:', error);
    }
  };

  const handleAddSpecialty = async () => {
    try {
      const newSpecialty = {
        doctorId: user.id,
        specialty: 'default',
        description: 'default',
        yearsOfExperience: 0
      };
      await doctorService.addSpecialty(newSpecialty);
      fetchSpecialties();
    } catch (error) {
      console.error('Error adding specialty:', error);
    }
  };

  return (
    <Card>
      <Card.Header className="d-flex justify-content-between align-items-center">
        <h4>Doctor Profile</h4>
        {!isEditingProfile && (
          <Button variant="outline-primary" onClick={() => setIsEditingProfile(true)}>
            Edit Profile
          </Button>
        )}
      </Card.Header>
      <Card.Body>
        <Row>
          <Col md={6}>
            {isEditingProfile ? (
              <Form>
                <Form.Group className="mb-3">
                  <Form.Label>First Name</Form.Label>
                  <Form.Control
                    type="text"
                    name="firstName"
                    value={profileData.firstName}
                    onChange={handleProfileChange}
                  />
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label>Last Name</Form.Label>
                  <Form.Control
                    type="text"
                    name="lastName"
                    value={profileData.lastName}
                    onChange={handleProfileChange}
                  />
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label>Email</Form.Label>
                  <Form.Control
                    type="email"
                    name="email"
                    value={profileData.email}
                    onChange={handleProfileChange}
                  />
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label>Phone</Form.Label>
                  <Form.Control
                    type="text"
                    name="phone"
                    value={profileData.phone}
                    onChange={handleProfileChange}
                  />
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label>Address</Form.Label>
                  <Form.Control
                    type="text"
                    name="address"
                    value={profileData.address}
                    onChange={handleProfileChange}
                  />
                </Form.Group>
                <Button variant="primary" onClick={handleSaveProfile}>Save Changes</Button>
                <Button variant="secondary" onClick={() => setIsEditingProfile(false)} className="ms-2">Cancel</Button>
              </Form>
            ) : (
              <>
                <p><strong>Name:</strong> {user?.firstName} {user?.lastName}</p>
                <p><strong>Email:</strong> {user?.email}</p>
                <p><strong>Phone:</strong> {user?.phone}</p>
                <p><strong>Address:</strong> {user?.address}</p>
              </>
            )}
          </Col>
        </Row>

        <h5 className="mt-4">Specialties</h5>
        {Array.isArray(specialties) && specialties.length > 0 ? (
          <ListGroup>
            {specialties.map((specialty) => (
              <ListGroup.Item key={specialty.id}>
                {editingSpecialty?.id === specialty.id ? (
                  <Form>
                    <Form.Group className="mb-3">
                      <Form.Label>Specialty</Form.Label>
                      <Form.Control
                        type="text"
                        value={editingSpecialty.specialty}
                        onChange={(e) => setEditingSpecialty({ ...editingSpecialty, specialty: e.target.value })}
                      />
                    </Form.Group>
                    <Form.Group className="mb-3">
                      <Form.Label>Description</Form.Label>
                      <Form.Control
                        as="textarea"
                        value={editingSpecialty.description}
                        onChange={(e) => setEditingSpecialty({ ...editingSpecialty, description: e.target.value })}
                      />
                    </Form.Group>
                    <Form.Group className="mb-3">
                      <Form.Label>Years of Experience</Form.Label>
                      <Form.Control
                        type="number"
                        value={editingSpecialty.yearsOfExperience}
                        onChange={(e) => setEditingSpecialty({ ...editingSpecialty, yearsOfExperience: parseInt(e.target.value) })}
                      />
                    </Form.Group>
                    <Button variant="primary" onClick={handleSaveSpecialty}>Save</Button>
                    <Button variant="secondary" onClick={() => setEditingSpecialty(null)} className="ms-2">Cancel</Button>
                  </Form>
                ) : (
                  <div>
                    <h6>{specialty.specialty}</h6>
                    <p>{specialty.description}</p>
                    <p><small>Years of Experience: {specialty.yearsOfExperience}</small></p>
                    <Button variant="outline-primary" onClick={() => handleEditSpecialty(specialty)} className="me-2">
                      Edit
                    </Button>
                    <Button variant="outline-danger" onClick={() => handleDeleteSpecialty(specialty.id)}>
                      Delete
                    </Button>
                  </div>
                )}
              </ListGroup.Item>
            ))}
          </ListGroup>
        ) : (
          <p>No specialties found. Add your first specialty below.</p>
        )}

        <Button 
          variant="success" 
          onClick={handleAddSpecialty}
          className="mt-3"
        >
          Add New Specialty
        </Button>
      </Card.Body>
    </Card>
  );
};

export default DoctorProfile; 