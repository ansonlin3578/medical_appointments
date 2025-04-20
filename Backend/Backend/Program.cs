using Backend.Models;
using System.Data;
using Backend.Repositories;
using Backend.Utils;
using Npgsql;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<AppointmentsRepository>();

var app = builder.Build();
// 在 app.Run(); 之前呼叫：
SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
// SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler()); // 雖然不用 TimeOnly，但預備可以加
SqlMapper.AddTypeHandler(new TimeSpanHandler());     // 必要
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();