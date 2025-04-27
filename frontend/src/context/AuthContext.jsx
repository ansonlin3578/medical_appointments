import React, { createContext, useState, useContext, useEffect } from 'react';
import authService from '../api/authService';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const token = localStorage.getItem('token');
        if (token) {
          const userData = await authService.getCurrentUser();
          console.log('Auth check - User data:', userData);
          setUser(userData);
        }
      } catch (error) {
        console.error('Auth check failed:', error);
        localStorage.removeItem('token');
      } finally {
        setLoading(false);
      }
    };

    checkAuth();
  }, []);

  const login = async (username, password) => {
    try {
      const response = await authService.login(username, password);
      console.log('Login response:', response);
      
      if (response && response.token) {
        localStorage.setItem('token', response.token);
        const userData = await authService.getCurrentUser();
        console.log('Current user data:', userData);
        
        if (!userData || !userData.role) {
          throw new Error('Invalid user data received from server');
        }
        
        setUser(userData);
        return userData;
      }
      throw new Error('Invalid response from server');
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  };

  const register = async (username, email, password, role, firstName, lastName) => {
    try {
      const response = await authService.register(username, email, password, role, firstName, lastName);
      if (response && response.token) {
        localStorage.setItem('token', response.token);
        const userData = await authService.getCurrentUser();
        setUser(userData);
        return userData;
      }
      throw new Error('Invalid response from server');
    } catch (error) {
      console.error('Registration error:', error);
      throw error;
    }
  };

  const logout = () => {
    authService.logout();
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, register, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};