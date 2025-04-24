import React from 'react';
import { Container, Row, Col, Card, Tab, Tabs, Button } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import UserManagement from '../admin/UserManagement';

const AdminDashboard = () => {
  const navigate = useNavigate();
  const { logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Container className="mt-4">
      <Row className="mb-3">
        <Col className="d-flex justify-content-end">
          <Button variant="outline-danger" onClick={handleLogout}>
            Logout
          </Button>
        </Col>
      </Row>
      <Row>
        <Col>
          <h2>Admin Dashboard</h2>
          <Tabs defaultActiveKey="users" className="mb-3">
            <Tab eventKey="users" title="User Management">
              <UserManagement />
            </Tab>
            <Tab eventKey="settings" title="System Settings">
              <Card>
                <Card.Body>
                  <Card.Title>System Settings</Card.Title>
                  <Card.Text>
                    System settings and configurations will be available here.
                  </Card.Text>
                </Card.Body>
              </Card>
            </Tab>
          </Tabs>
        </Col>
      </Row>
    </Container>
  );
};

export default AdminDashboard; 