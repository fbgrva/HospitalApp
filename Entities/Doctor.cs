namespace HospitalApp.Entities
{
    public class Doctor : BaseEntity
    {
        public string Specialization { get; set; }
        public int Experience { get; set; }

        public int UserId { get; set; }
        public int DepartmentId { get; set; }

        public User User { get; set; }

        public Department Department { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}