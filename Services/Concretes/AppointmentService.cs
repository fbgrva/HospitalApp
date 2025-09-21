using HospitalApp.Entities;
using HospitalApp.Services.Abstracts;
using HospitalApp.DBContextHospital;
using Microsoft.EntityFrameworkCore;

namespace HospitalApp.Services.Concretes
{
    public class AppointmentService : BaseService, IAppointmentService
    {
        public AppointmentService(HospitalDbContext context) : base(context)
        {
        }

        public Appointment Add(Appointment appointment)
        {
            
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            if (!_context.Patients.Any(p => p.Id == appointment.PatientId && !p.IsDeleted))
                throw new Exception("Patient not found");

            if (!_context.Doctors.Any(d => d.Id == appointment.DoctorId && !d.IsDeleted))
                throw new Exception("Doctor not found");

            if (!CheckDoctorAvailability(appointment.DoctorId, appointment.AppointmentDate))
                throw new Exception("Doctor is not available at this time");

            if (_context.Appointments.Any(a => a.PatientId == appointment.PatientId &&
                                             a.AppointmentDate == appointment.AppointmentDate &&
                                             !a.IsDeleted))
                throw new Exception("Patient already has an appointment at this time");

            appointment.Status = "Scheduled"; 
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
            return appointment;
        }

        public Appointment Update(Appointment appointment)
        {
            if (appointment == null)
                throw new ArgumentNullException(nameof(appointment));

            var existingAppointment = _context.Appointments.Find(appointment.Id);
            if (existingAppointment == null || existingAppointment.IsDeleted)
                throw new Exception("Appointment not found");

            if (existingAppointment.AppointmentDate != appointment.AppointmentDate)
            {
                if (!CheckDoctorAvailability(appointment.DoctorId, appointment.AppointmentDate))
                    throw new Exception("Doctor is not available at this time");
            }

            existingAppointment.AppointmentDate = appointment.AppointmentDate;
            existingAppointment.Status = appointment.Status;
            existingAppointment.UpdatedDate = DateTime.Now;

            _context.Appointments.Update(existingAppointment);
            _context.SaveChanges();
            return existingAppointment;
        }

        public void Delete(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null && !appointment.IsDeleted)
            {
                appointment.IsDeleted = true;
                appointment.DeletedDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public Appointment GetById(int id)
        {
            return _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Department)
                .FirstOrDefault(a => a.Id == id && !a.IsDeleted);
        }

        public List<Appointment> GetAll()
        {
            return _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Department)
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        public List<Appointment> GetByPatientId(int patientId)
        {
            return _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Department)
                .Where(a => a.PatientId == patientId && !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        public List<Appointment> GetByDoctorId(int doctorId)
        {
            return _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Where(a => a.DoctorId == doctorId && !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        public List<Appointment> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Department)
                .Where(a => a.AppointmentDate >= startDate &&
                           a.AppointmentDate <= endDate &&
                           !a.IsDeleted)
                .OrderBy(a => a.AppointmentDate)
                .ToList();
        }

        public List<Appointment> GetByStatus(string status)
        {
            return _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Department)
                .Where(a => a.Status == status && !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();
        }

        public bool CheckDoctorAvailability(int doctorId, DateTime appointmentDate)
        {
            var existingAppointment = _context.Appointments
                .FirstOrDefault(a => a.DoctorId == doctorId &&
                                   a.AppointmentDate == appointmentDate &&
                                   !a.IsDeleted);

            return existingAppointment == null;
        }

        public List<DateTime> GetAvailableSlots(int doctorId, DateTime date)
        {
            var availableSlots = new List<DateTime>();
            var startTime = new DateTime(date.Year, date.Month, date.Day, 9, 0, 0); 
            var endTime = new DateTime(date.Year, date.Month, date.Day, 17, 0, 0);  

            var appointments = _context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                           a.AppointmentDate.Date == date.Date &&
                           !a.IsDeleted)
                .ToList();

            for (var time = startTime; time < endTime; time = time.AddMinutes(30))
            {
                if (!appointments.Any(a => a.AppointmentDate == time))
                {
                    availableSlots.Add(time);
                }
            }

            return availableSlots;
        }

    }
}