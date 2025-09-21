using HospitalApp.Entities;
using HospitalApp.Services.Abstracts;
using HospitalApp.DBContextHospital;
using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Services.Concretes
{
    public class DepartmentService : BaseService, IDepartmentService
    {
        public DepartmentService(HospitalDbContext context) : base(context)
        {
        }

        public Department Add(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            if (_context.Departments.Any(d => d.Name == department.Name && !d.IsDeleted))
                throw new Exception("Department with this name already exists");

            _context.Departments.Add(department);
            _context.SaveChanges();
            return department;
        }

        public Department Update(Department department)
        {
            if (department == null)
                throw new ArgumentNullException(nameof(department));

            var existingDepartment = _context.Departments.Find(department.Id);
            if (existingDepartment == null || existingDepartment.IsDeleted)
                throw new Exception("Department not found");

            if (_context.Departments.Any(d => d.Name == department.Name && d.Id != department.Id && !d.IsDeleted))
                throw new Exception("Department with this name already exists");

            existingDepartment.Name = department.Name;
            existingDepartment.Description = department.Description;
            existingDepartment.UpdatedDate = DateTime.Now;

            _context.Departments.Update(existingDepartment);
            _context.SaveChanges();
            return existingDepartment;
        }

        public void Delete(int id)
        {
            var department = _context.Departments.Find(id);
            if (department != null && !department.IsDeleted)
            {
                if (_context.Doctors.Any(d => d.DepartmentId == id && !d.IsDeleted))
                    throw new Exception("Cannot delete department with assigned doctors");

                department.IsDeleted = true;
                department.DeletedDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public Department GetById(int id)
        {
            return _context.Departments
                .Include(d => d.Doctors)
                    .ThenInclude(doc => doc.User)
                .FirstOrDefault(d => d.Id == id && !d.IsDeleted);
        }

        public List<Department> GetAll()
        {
            return _context.Departments
                .Include(d => d.Doctors)
                    .ThenInclude(doc => doc.User)
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToList();
        }

        public List<Department> GetByName(string name)
        {
            return _context.Departments
                .Include(d => d.Doctors)
                    .ThenInclude(doc => doc.User)
                .Where(d => d.Name.Contains(name) && !d.IsDeleted)
                .OrderBy(d => d.Name)
                .ToList();
        }

        public int GetDoctorsCount(int departmentId)
        {
            return _context.Doctors
                .Count(d => d.DepartmentId == departmentId && !d.IsDeleted);
        }

    }
}