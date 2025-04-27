import React, { useState, useEffect } from 'react';
import { Table, Button, Modal, Form, Alert } from 'react-bootstrap';
import axios from 'axios';

const UserManagement = () => {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await axios.get('http://localhost:5000/api/admin/users', {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      setUsers(response.data);
      setLoading(false);
    } catch (err) {
      setError('Failed to fetch users');
      setLoading(false);
    }
  };

  const handleMoveToPatient = async (user) => {
    try {
      const token = localStorage.getItem('token');
      await axios.post(`http://localhost:5000/api/admin/move-to-patient/${user.id}`, {}, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      fetchUsers(); // Refresh the user list
      setShowModal(false);
    } catch (err) {
      setError('Failed to move user to patient table');
    }
  };

  const handleShowModal = (user) => {
    setSelectedUser(user);
    setShowModal(true);
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <h2>User Management</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      
      <Table striped bordered hover>
        <thead>
          <tr>
            <th>ID</th>
            <th>Username</th>
            <th>Email</th>
            <th>Role</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {users.map(user => (
            <tr key={user.id}>
              <td>{user.id}</td>
              <td>{user.username}</td>
              <td>{user.email}</td>
              <td>{user.role}</td>
              <td>
                {user.role === 'Patient' && !user.isInPatientTable && (
                  <Button 
                    variant="primary" 
                    onClick={() => handleShowModal(user)}
                  >
                    Move to Patient Table
                  </Button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Confirm Action</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          Are you sure you want to move {selectedUser?.username} to the patient table?
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>
            Cancel
          </Button>
          <Button variant="primary" onClick={() => handleMoveToPatient(selectedUser)}>
            Confirm
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default UserManagement; 