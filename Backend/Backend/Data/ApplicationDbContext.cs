using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorSpecialty> DoctorSpecialties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 設定 User 表的唯一索引
            // Username 和 Email 必須唯一，因為它們用於用戶登錄和識別
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 設定 Patient 和 User 的關聯
            // 一個病人必須對應一個用戶帳號，當用戶被刪除時，相關的病人記錄也會被刪除
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 設定 DoctorSchedule 和 User 的關聯
            // 一個排班必須對應一個醫生（User表中Role為Hospital的用戶）
            // 當醫生被刪除時，其排班記錄也會被刪除
            modelBuilder.Entity<DoctorSchedule>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // 設定 Appointment 和 Patient 的關聯
            // 一個預約必須對應一個病人，當病人被刪除時，其預約記錄也會被刪除
            modelBuilder.Entity<Appointment>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // 設定 Appointment 和 Doctor 的關聯
            // 一個預約必須對應一個醫生（User表中Role為Doctor的用戶）
            // 當醫生被刪除時，其預約記錄也會被刪除
            modelBuilder.Entity<Appointment>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置 User 和 DoctorSchedule 的關係
            // 一個醫生（User表中Role為Hospital的用戶）可以有多個排班
            // 每個排班必須屬於一個醫生
            // 當醫生被刪除時，其所有排班記錄也會被刪除
            modelBuilder.Entity<User>()
                .HasMany(u => u.DoctorSchedules)
                .WithOne()
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // 配置 User 和 DoctorSpecialty 的關係
            // 一個醫生（User表中Role為Hospital的用戶）可以有多個專業領域
            // 每個專業領域記錄必須屬於一個醫生
            // 當醫生被刪除時，其所有專業領域記錄也會被刪除
            modelBuilder.Entity<User>()
                .HasMany(u => u.DoctorSpecialties)
                .WithOne(ds => ds.Doctor)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 