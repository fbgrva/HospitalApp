using HospitalApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalApp.Configurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.HasKey(mr => mr.Id);

            builder.Property(mr => mr.Diagnosis)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(mr => mr.Treatment)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(mr => mr.RecordDate)
                   .IsRequired();

            builder.HasOne(mr => mr.Patient)
                   .WithMany(p => p.MedicalRecords)
                   .HasForeignKey(mr => mr.PatientId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(mr => mr.Doctor)
                   .WithMany(d => d.MedicalRecords)
                   .HasForeignKey(mr => mr.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
