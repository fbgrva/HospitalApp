using HospitalApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalApp.DBContextHospital
{
    public class HospitalDbContext : DbContext
    {
        // Parameterless constructor (migrations üçün lazımdır)
        public HospitalDbContext() { }

        // DbContextOptions qəbul edən constructor (ƏSAS)
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Əgər onsuz da konfiqurasiya edilməyibsə, bu connection stringi işləd
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=AUCE;Initial Catalog=HospitalApp;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True");
            }
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiqurasiyaları tətbiq et
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HospitalDbContext).Assembly);
        }
    }
}