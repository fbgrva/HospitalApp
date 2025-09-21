using HospitalApp.Entities;

namespace HospitalApp.Services.Abstracts
{
    public interface IMedicalRecordService
    {
        MedicalRecord Add(MedicalRecord medicalRecord);
        MedicalRecord Update(MedicalRecord medicalRecord);
        void Delete(int id);
        MedicalRecord GetById(int id);
        List<MedicalRecord> GetAll();
        List<MedicalRecord> GetByPatientId(int patientId);
        List<MedicalRecord> GetByDoctorId(int doctorId);
    }
}