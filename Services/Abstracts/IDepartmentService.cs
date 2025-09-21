using HospitalApp.Entities;

namespace HospitalApp.Services.Abstracts
{
    public interface IDepartmentService
    {
        Department Add(Department department);
        Department Update(Department department);
        void Delete(int id);
        Department GetById(int id);
        List<Department> GetAll();
    }
}