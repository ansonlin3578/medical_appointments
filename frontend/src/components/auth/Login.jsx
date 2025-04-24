import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { Form, Button, Card, Container, Alert, Row, Col } from 'react-bootstrap';
import { useAuth } from '../../context/AuthContext';
import Roles from '../../constants/roles';

const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const userData = await login(username, password);
      console.log('Login response:', userData);
      console.log('User role:', userData.role);
      console.log('Roles.PATIENT:', Roles.PATIENT);

      // Convert role to proper case for comparison
      const normalizedRole = userData.role.charAt(0).toUpperCase() + userData.role.slice(1).toLowerCase();

      // Redirect based on user role
      if (normalizedRole === Roles.DOCTOR) {
        navigate('/doctor-dashboard');
      } else if (normalizedRole === Roles.PATIENT) {
        navigate('/patient-dashboard');
      } else if (normalizedRole === Roles.ADMIN) {
        navigate('/admin-dashboard');
      } else {
        setError(`Invalid user role: ${userData.role}`);
      }
    } catch (err) {
      console.error('Login error:', err);
      setError('Failed to log in. Please check your credentials.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container style={{ maxWidth: '600px' }}>
      <Row className="justify-content-center">
        <Col>
          <Card className="shadow-lg">
            <Card.Body className="p-5">
              <div className="text-center mb-4">
                <h2 className="fw-bold">Welcome Back</h2>
                <p className="text-muted">Please sign in to your account</p>
              </div>

              {error && (
                <Alert variant="danger" className="mb-4">
                  {error}
                </Alert>
              )}

              <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-4">
                  <Form.Label>Username</Form.Label>
                  <Form.Control
                    type="text"
                    placeholder="Enter your username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                    size="lg"
                  />
                </Form.Group>

                <Form.Group className="mb-4">
                  <Form.Label>Password</Form.Label>
                  <Form.Control
                    type="password"
                    placeholder="Enter your password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                    size="lg"
                  />
                </Form.Group>

                <Button
                  variant="primary"
                  type="submit"
                  className="w-100 mb-3 py-3"
                  disabled={loading}
                  size="lg"
                >
                  {loading ? 'Signing in...' : 'Sign In'}
                </Button>

                <div className="text-center">
                  <p className="mb-0">
                    Don't have an account?{' '}
                    <Link to="/register" className="text-primary">
                      Sign up
                    </Link>
                  </p>
                </div>
              </Form>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default Login;