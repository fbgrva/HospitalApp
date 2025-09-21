using HospitalApp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalApp.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(d => d.Description)
                   .HasMaxLength(500)
                   .IsRequired(false);

            builder.HasMany(d => d.Doctors)
                   .WithOne(doc => doc.Department)
                   .HasForeignKey(doc => doc.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
