require('dotenv').config();
const express = require('express');
const cors = require('cors');
const { Pool } = require('pg');

const app = express();

// Middleware
// app.use(cors({
//   origin: 'http://localhost:3000',
//   methods: ['GET', 'POST'],
//   credentials: false
// }));
app.use(express.json());

// Log middleware (for debugging)
app.use((req, res, next) => {
  console.log(`[${req.method}] ${req.url}`);
  next();
});

// PostgreSQL pool
const pool = new Pool({
  user: process.env.PG_USER,
  host: process.env.PG_HOST,
  database: process.env.PG_DB,
  password: process.env.PG_PASS,
  port: process.env.PG_PORT,
});

pool.connect()
  .then(() => console.log('✅ Connected to PostgreSQL'))
  .catch((err) => {
    console.error('❌ PostgreSQL connection error:', err);
    process.exit(1);
  });

// API routes
app.get('/api/ping', (req, res) => {
  res.json({ msg: 'pong' });
});

    app.get('/api/doctors', async (req, res) => {
        try {
        const result = await pool.query('SELECT * FROM doctors ORDER BY id');
        res.json(result.rows);
        } catch (error) {
        console.error('DB error (doctors):', error);
        res.status(500).json({ error: 'Database error' });
        }
    });
  
    app.get('/api/appointments', async (req, res) => {
        try {
          const result = await pool.query(`
            SELECT 
            appointments.id,
            appointments.name,
            appointments.doctor_id,
            TO_CHAR(appointments.date, 'YYYY-MM-DD') AS date,
            TO_CHAR(appointments.start_time, 'HH24:MI') AS start_time,
            TO_CHAR(appointments.end_time, 'HH24:MI') AS end_time,
            doctors.name AS doctor_name
            FROM appointments
            JOIN doctors ON appointments.doctor_id = doctors.id
            ORDER BY date ASC, start_time ASC;
          `);
          res.json(result.rows);
        } catch (error) {
          console.error('DB error:', error);
          res.status(500).json({ error: 'Database error' });
        }
      });

      app.get('/api/appointments/occupied', async (req, res) => {
        const { doctor_id, date } = req.query;
      
        if (!doctor_id || !date) {
          return res.status(400).json({ error: '缺少 doctor_id 或 date' });
        }
      
        try {
          const result = await pool.query(
            `SELECT TO_CHAR(start_time, 'HH24:MI') AS start_time
             FROM appointments
             WHERE doctor_id = $1 AND date = $2`,
            [doctor_id, date]
          );
          const occupiedTimes = result.rows.map(row => row.start_time);
          res.json({ occupiedTimes });
        } catch (error) {
          console.error('Occupied check error:', error);
          res.status(500).json({ error: 'Database error' });
        }
      });
      
      
      app.post('/api/appointments', async (req, res) => {
        const { name, date, start_time, end_time, doctor_id } = req.body;
      
        try {
          // 時間重疊檢查
          const conflictCheck = await pool.query(
            `SELECT * FROM appointments
             WHERE doctor_id = $1 AND date = $2
               AND NOT ($3 >= end_time OR $4 <= start_time)`,
            [doctor_id, date, start_time, end_time]
          );
      
          if (conflictCheck.rows.length > 0) {
            return res.status(400).json({ error: '時間衝突，請選擇其他時段' });
          }
      
          await pool.query(
            `INSERT INTO appointments (name, date, start_time, end_time, doctor_id)
             VALUES ($1, $2, $3, $4, $5)`,
            [name, date, start_time, end_time, doctor_id]
          );
      
          res.status(201).json({ msg: 'Appointment added' });
      
        } catch (error) {
          console.error('Insert error:', error);
          res.status(500).json({ error: 'Database error' });
        }
      });

      app.put('/api/appointments/:id', async (req, res) => {
        const { id } = req.params;
        const { date, start_time, end_time, doctor_id } = req.body;
      
        try {
          // 檢查衝突
          const conflictCheck = await pool.query(
            `SELECT * FROM appointments
             WHERE doctor_id = $1 AND date = $2
               AND NOT ($3 >= end_time OR $4 <= start_time)
               AND id != $5`,
            [doctor_id, date, start_time, end_time, id]
          );
      
          if (conflictCheck.rows.length > 0) {
            return res.status(400).json({ error: '時間衝突，請選擇其他時段' });
          }
      
          await pool.query(
            `UPDATE appointments
             SET date = $1, start_time = $2, end_time = $3, doctor_id = $4
             WHERE id = $5`,
            [date, start_time, end_time, doctor_id, id]
          );
      
          res.json({ msg: 'Appointment updated' });
      
        } catch (error) {
          console.error('Update error:', error);
          res.status(500).json({ error: 'Database error' });
        }
      });
      
    app.delete('/api/appointments/:id', async (req, res) => {
        const { id } = req.params;
        try {
            await pool.query('DELETE FROM appointments WHERE id = $1', [id]);
            res.json({ msg: 'Appointment deleted' });
        } catch (error) {
            console.error('Delete error:', error);
            res.status(500).json({ error: 'Database error' });
        }
    });
    
    const PORT = process.env.PORT || 5000;
        app.listen(PORT, () => {
        console.log(`🚀 Server running on port ${PORT}`);
    });