using HospitalApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalApp.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Specialization)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.Experience)
                   .IsRequired();

            builder.HasOne(d => d.User)
                   .WithOne(u => u.Doctor)
                   .HasForeignKey<Doctor>(d => d.UserId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(d => d.Department)
                   .WithMany(dept => dept.Doctors)
                   .HasForeignKey(d => d.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(d => d.Appointments)
                   .WithOne(a => a.Doctor)
                   .HasForeignKey(a => a.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(d => d.MedicalRecords)
                   .WithOne(mr => mr.Doctor)
                   .HasForeignKey(mr => mr.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
