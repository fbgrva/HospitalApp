namespace HospitalApp.Entities
{
    public class MedicalRecord : BaseEntity
    {
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public DateTime RecordDate { get; set; }

        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}