using HospitalApp.Entities;

namespace HospitalApp.Services.Abstracts
{
    public interface IPatientService
    {
        Patient Add(Patient patient);
        Patient Update(Patient patient);
        void Delete(int id);
        Patient GetById(int id);
        List<Patient> GetAll();
        Patient GetByUserId(int userId);
    }
}