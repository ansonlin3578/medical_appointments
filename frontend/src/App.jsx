import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { useAuth } from './context/AuthContext';
import Login from './components/auth/Login';
import Register from './components/auth/Register';
import DoctorDashboard from './components/dashboard/DoctorDashboard';
import PatientDashboard from './components/dashboard/PatientDashboard';
import AdminDashboard from './components/dashboard/AdminDashboard';
import Roles from './constants/roles';
import 'bootstrap/dist/css/bootstrap.min.css';

const PrivateRoute = ({ children, requiredRole }) => {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  if (!user) {
    return <Navigate to="/login" />;
  }

  // Convert both roles to lowercase for case-insensitive comparison
  const userRole = user.role?.toLowerCase();
  const requiredRoleLower = requiredRole?.toLowerCase();

  if (requiredRole && userRole !== requiredRoleLower) {
    console.log('Role mismatch:', { userRole, requiredRoleLower });
    return <Navigate to="/" />;
  }

  return React.cloneElement(children, { user });
};

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route
            path="/doctor-dashboard"
            element={
              <PrivateRoute requiredRole={Roles.DOCTOR}>
                <DoctorDashboard />
              </PrivateRoute>
            }
          />
          <Route
            path="/patient-dashboard"
            element={
              <PrivateRoute requiredRole={Roles.PATIENT}>
                <PatientDashboard />
              </PrivateRoute>
            }
          />
          <Route
            path="/admin-dashboard"
            element={
              <PrivateRoute requiredRole={Roles.ADMIN}>
                <AdminDashboard />
              </PrivateRoute>
            }
          />
          <Route path="/" element={<Navigate to="/login" />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;