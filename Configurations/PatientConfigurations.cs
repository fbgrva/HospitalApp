using HospitalApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalApp.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Address)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(p => p.DateOfBirth)
                   .IsRequired();

            builder.Property(p => p.Gender)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.HasOne(p => p.User)
                   .WithOne(u => u.Patient)
                   .HasForeignKey<Patient>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(p => p.Appointments)
                   .WithOne(a => a.Patient)
                   .HasForeignKey(a => a.PatientId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(p => p.MedicalRecords)
                   .WithOne(mr => mr.Patient)
                   .HasForeignKey(mr => mr.PatientId)
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
