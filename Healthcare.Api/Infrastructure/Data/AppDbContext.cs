using Healthcare.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Healthcare.Api.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<DoctorSchedules> DoctorSchedules => Set<DoctorSchedules>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Doctor>()
                .HasKey(a => a.DoctorID);

            modelBuilder.Entity<DoctorSchedules>()
                .HasKey(a => a.ScheduleID);

            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.AppointmentID);

            //Appointment concurrency
            modelBuilder.Entity<Appointment>()
                .Property(a => a.RowVersion)
                .IsRowVersion();

            base.OnModelCreating(modelBuilder);
        }
    }
}