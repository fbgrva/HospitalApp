using HospitalApp.Entities;

namespace HospitalApp.Services.Abstracts
{
    public interface IDoctorService
    {
        Doctor Add(Doctor doctor);
        Doctor Update(Doctor doctor);
        void Delete(int id);
        Doctor GetById(int id);
        List<Doctor> GetAll();
        Doctor GetByUserId(int userId);
        List<Doctor> GetByDepartmentId(int departmentId);
    }
}