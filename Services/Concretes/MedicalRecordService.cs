using HospitalApp.Entities;
using HospitalApp.Services.Abstracts;
using HospitalApp.DBContextHospital;
using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Services.Concretes
{
    public class MedicalRecordService : BaseService, IMedicalRecordService
    {
        public MedicalRecordService(HospitalDbContext context) : base(context)
        {
        }

        public MedicalRecord Add(MedicalRecord medicalRecord)
        {
            if (medicalRecord == null)
                throw new ArgumentNullException(nameof(medicalRecord));

            if (!_context.Patients.Any(p => p.Id == medicalRecord.PatientId && !p.IsDeleted))
                throw new Exception("Patient not found");

            if (!_context.Doctors.Any(d => d.Id == medicalRecord.DoctorId && !d.IsDeleted))
                throw new Exception("Doctor not found");

            _context.MedicalRecords.Add(medicalRecord);
            _context.SaveChanges();
            return medicalRecord;
        }

        public MedicalRecord Update(MedicalRecord medicalRecord)
        {
            if (medicalRecord == null)
                throw new ArgumentNullException(nameof(medicalRecord));

            var existingRecord = _context.MedicalRecords.Find(medicalRecord.Id);
            if (existingRecord == null || existingRecord.IsDeleted)
                throw new Exception("Medical record not found");

            existingRecord.Diagnosis = medicalRecord.Diagnosis;
            existingRecord.Treatment = medicalRecord.Treatment;
            existingRecord.RecordDate = medicalRecord.RecordDate;
            existingRecord.UpdatedDate = DateTime.Now;

            _context.MedicalRecords.Update(existingRecord);
            _context.SaveChanges();
            return existingRecord;
        }

        public void Delete(int id)
        {
            var medicalRecord = _context.MedicalRecords.Find(id);
            if (medicalRecord != null && !medicalRecord.IsDeleted)
            {
                medicalRecord.IsDeleted = true;
                medicalRecord.DeletedDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public MedicalRecord GetById(int id)
        {
            return _context.MedicalRecords
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.Department)
                .FirstOrDefault(mr => mr.Id == id && !mr.IsDeleted);
        }

        public List<MedicalRecord> GetAll()
        {
            return _context.MedicalRecords
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.Department)
                .Where(mr => !mr.IsDeleted)
                .OrderByDescending(mr => mr.RecordDate)
                .ToList();
        }

        public List<MedicalRecord> GetByPatientId(int patientId)
        {
            return _context.MedicalRecords
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.Department)
                .Where(mr => mr.PatientId == patientId && !mr.IsDeleted)
                .OrderByDescending(mr => mr.RecordDate)
                .ToList();
        }

        public List<MedicalRecord> GetByDoctorId(int doctorId)
        {
            return _context.MedicalRecords
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.Department)
                .Where(mr => mr.DoctorId == doctorId && !mr.IsDeleted)
                .OrderByDescending(mr => mr.RecordDate)
                .ToList();
        }

        public List<MedicalRecord> GetByPatientAndDoctor(int patientId, int doctorId)
        {
            return _context.MedicalRecords
                .Include(mr => mr.Patient)
                    .ThenInclude(p => p.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.User)
                .Include(mr => mr.Doctor)
                    .ThenInclude(d => d.Department)
                .Where(mr => mr.PatientId == patientId &&
                             mr.DoctorId == doctorId &&
                             !mr.IsDeleted)
                .OrderByDescending(mr => mr.RecordDate)
                .ToList();
        }

    }
}