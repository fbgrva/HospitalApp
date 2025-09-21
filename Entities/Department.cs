namespace HospitalApp.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
    }
}