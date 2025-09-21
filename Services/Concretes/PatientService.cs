using HospitalApp.Entities;
using HospitalApp.Services.Abstracts;
using HospitalApp.DBContextHospital;
using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Services.Concretes
{
    public class PatientService : BaseService, IPatientService
    {
        public PatientService(HospitalDbContext context) : base(context)
        {
        }

        public Patient Add(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
            return patient;
        }

        public Patient Update(Patient patient)
        {
            _context.Patients.Update(patient);
            _context.SaveChanges();
            return patient;
        }

        public void Delete(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                patient.IsDeleted = true;
                patient.DeletedDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public Patient GetById(int id)
        {
            return _context.Patients
                .Include(p => p.User)
                .FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        }

        public List<Patient> GetAll()
        {
            return _context.Patients
                .Include(p => p.User)
                .Where(p => !p.IsDeleted)
                .ToList();
        }

        public Patient GetByUserId(int userId)
        {
            return _context.Patients
                .Include(p => p.User)
                .FirstOrDefault(p => p.UserId == userId && !p.IsDeleted);
        }
    }
}