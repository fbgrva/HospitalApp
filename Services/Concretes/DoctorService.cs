using HospitalApp.Entities;
using HospitalApp.Services.Abstracts;
using HospitalApp.DBContextHospital;
using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Services.Concretes
{
    public class DoctorService : BaseService, IDoctorService
    {
        public DoctorService(HospitalDbContext context) : base(context)
        {
        }

        public Doctor Add(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            _context.SaveChanges();
            return doctor;
        }

        public Doctor Update(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            _context.SaveChanges();
            return doctor;
        }

        public void Delete(int id)
        {
            var doctor = _context.Doctors.Find(id);
            if (doctor != null)
            {
                doctor.IsDeleted = true;
                doctor.DeletedDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public Doctor GetById(int id)
        {
            return _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefault(d => d.Id == id && !d.IsDeleted);
        }

        public List<Doctor> GetAll()
        {
            return _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .Where(d => !d.IsDeleted)
                .ToList();
        }

        public Doctor GetByUserId(int userId)
        {
            return _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefault(d => d.UserId == userId && !d.IsDeleted);
        }

        public List<Doctor> GetByDepartmentId(int departmentId)
        {
            return _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .Where(d => d.DepartmentId == departmentId && !d.IsDeleted)
                .ToList();
        }
    }
}