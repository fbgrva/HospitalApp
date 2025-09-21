namespace HospitalApp.Entities
{
    public class Appointment : BaseEntity
    {
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } 

        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}