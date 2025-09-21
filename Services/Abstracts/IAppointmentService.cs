using HospitalApp.Entities;

namespace HospitalApp.Services.Abstracts
{
    public interface IAppointmentService
    {
        Appointment Add(Appointment appointment);
        Appointment Update(Appointment appointment);
        void Delete(int id);
        Appointment GetById(int id);
        List<Appointment> GetAll();
        List<Appointment> GetByPatientId(int patientId);
        List<Appointment> GetByDoctorId(int doctorId);
        List<Appointment> GetByDateRange(DateTime startDate, DateTime endDate);
    }
}