using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Backend.Data;

namespace Backend.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250424160000_RemoveDoctorsTable")]
    partial class RemoveDoctorsTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Backend.Models.Appointment", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<DateTime>("AppointmentTime")
                    .HasColumnType("timestamp with time zone");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int>("DoctorId")
                    .HasColumnType("integer");

                b.Property<int>("PatientId")
                    .HasColumnType("integer");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnType("timestamp with time zone");

                b.HasKey("Id");

                b.ToTable("Appointments");
            });

            modelBuilder.Entity("Backend.Models.DoctorSchedule", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int>("DayOfWeek")
                    .HasColumnType("integer");

                b.Property<int>("DoctorId")
                    .HasColumnType("integer");

                b.Property<TimeSpan>("EndTime")
                    .HasColumnType("interval");

                b.Property<TimeSpan>("StartTime")
                    .HasColumnType("interval");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnType("timestamp with time zone");

                b.HasKey("Id");

                b.ToTable("DoctorSchedules");
            });

            modelBuilder.Entity("Backend.Models.DoctorSpecialty", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<int>("DoctorId")
                    .HasColumnType("integer");

                b.Property<string>("Specialty")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int>("YearsOfExperience")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.ToTable("DoctorSpecialties");
            });

            modelBuilder.Entity("Backend.Models.Patient", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<DateTime>("DateOfBirth")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Gender")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("MedicalHistory")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<int>("UserId")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.ToTable("Patients");
            });

            modelBuilder.Entity("Backend.Models.User", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<string>("Address")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("FirstName")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("LastName")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("PasswordHash")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Phone")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Role")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnType("timestamp with time zone");

                b.Property<string>("Username")
                    .IsRequired()
                    .HasColumnType("text");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique();

                b.HasIndex("Username")
                    .IsUnique();

                b.ToTable("Users");
            });
        }
    }
} 