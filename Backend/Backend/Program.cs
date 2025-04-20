using Backend.Models;
using System.Data;
using Backend.Repositories;
using Backend.Utils;
using Npgsql;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// 設定 CORS 規則
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // 允許 React 本地開發環境
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

// 加這行來啟用 CORS
app.UseCors();
// 路由 & 授權
app.UseAuthorization();
app.MapControllers();
app.Run();