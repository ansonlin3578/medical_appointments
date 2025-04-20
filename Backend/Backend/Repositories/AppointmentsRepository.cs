using Dapper;
using System.Data;
using Backend.Models;

namespace Backend.Repositories;

public class AppointmentsRepository
{
    private readonly IDbConnection _db;

    public AppointmentsRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        // language=PostgreSQL
        var sql = "SELECT * FROM appointments_v2 ORDER BY date, start_time";
        return await _db.QueryAsync<Appointment>(sql);
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        // language=PostgreSQL
        var sql = "SELECT * FROM appointments_v2 WHERE id = @Id";
        return await _db.QueryFirstOrDefaultAsync<Appointment>(sql, new { Id = id });
    }

    public async Task CreateAsync(Appointment a)
    {
        // language=PostgreSQL
        var sql = @"
            INSERT INTO appointments_v2 (id, name, date, start_time, end_time, doctor_id)
            VALUES (@Id, @Name, @Date, @StartTime, @EndTime, @DoctorId)";
        a.Id = Guid.NewGuid();
        Console.WriteLine($"➡ Creating appointment for {a.Name} with doctor {a.DoctorId}");
        await _db.ExecuteAsync(sql, a);
    }

    public async Task UpdateAsync(Appointment a)
    {
        // language=PostgreSQL
        var sql = @"
            UPDATE appointments_v2 SET
                name = @Name,
                date = @Date,
                start_time = @StartTime,
                end_time = @EndTime,
                doctor_id = @DoctorId
            WHERE id = @Id";
        await _db.ExecuteAsync(sql, a);
    }

    public async Task DeleteAsync(Guid id)
    {
        // language=PostgreSQL
        var sql = "DELETE FROM appointments_v2 WHERE id = @Id";
        await _db.ExecuteAsync(sql, new { Id = id });
    }
}
