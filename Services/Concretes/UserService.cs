using HospitalApp.Entities;
using HospitalApp.Services.Abstracts;
using HospitalApp.DBContextHospital;
using HospitalApp.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HospitalApp.Services.Concretes
{
    public class UserService : BaseService, IUserService
    {
        private readonly EmailService _emailService;
        private readonly OTPService _otpService;

        public UserService(HospitalDbContext context, EmailService emailService, OTPService otpService)
            : base(context)
        {
            _emailService = emailService;
            _otpService = otpService;
        }

        public User Add(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (_context.Users.Any(u => u.Username == user.Username && !u.IsDeleted))
                throw new Exception("İstifadəçi adı artıq mövcuddur");

            if (_context.Users.Any(u => u.Email == user.Email && !u.IsDeleted))
                throw new Exception("Email artıq mövcuddur");

            user.Password = HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public User Update(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var existingUser = _context.Users.Find(user.Id);
            if (existingUser == null || existingUser.IsDeleted)
                throw new Exception("İstifadəçi tapılmadı");

            if (_context.Users.Any(u => u.Username == user.Username && u.Id != user.Id && !u.IsDeleted))
                throw new Exception("İstifadəçi adı artıq mövcuddur");

            if (_context.Users.Any(u => u.Email == user.Email && u.Id != user.Id && !u.IsDeleted))
                throw new Exception("Email artıq mövcuddur");

            existingUser.Name = user.Name;
            existingUser.Surname = user.Surname;
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.UpdatedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(user.Password) && user.Password != existingUser.Password)
            {
                existingUser.Password = HashPassword(user.Password);
            }

            _context.Users.Update(existingUser);
            _context.SaveChanges();
            return existingUser;
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null && !user.IsDeleted)
            {
                if (_context.Patients.Any(p => p.UserId == id && !p.IsDeleted) ||
                    _context.Doctors.Any(d => d.UserId == id && !d.IsDeleted))
                {
                    throw new Exception("İstifadəçinin xəstə və ya həkim məlumatları var. Əvvəlcə onları silin.");
                }

                user.IsDeleted = true;
                user.DeletedDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public User GetById(int id)
        {
            return _context.Users
                .FirstOrDefault(u => u.Id == id && !u.IsDeleted);
        }

        public List<User> GetAll()
        {
            return _context.Users
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.Name)
                .ThenBy(u => u.Surname)
                .ToList();
        }

        public User GetByUsername(string username)
        {
            return _context.Users
                .FirstOrDefault(u => u.Username == username && !u.IsDeleted);
        }

        public User GetByEmail(string email)
        {
            return _context.Users
                .FirstOrDefault(u => u.Email == email && !u.IsDeleted);
        }

        public User Register(User user, string password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(password) || password.Length < 6)
                throw new Exception("Şifrə ən az 6 simvol olmalıdır");

            if (_context.Users.Any(u => u.Username == user.Username && !u.IsDeleted))
                throw new Exception("İstifadəçi adı artıq mövcuddur");

            if (_context.Users.Any(u => u.Email == user.Email && !u.IsDeleted))
                throw new Exception("Email artıq mövcuddur");

            user.OTP = _otpService.GenerateOTP();
            user.OTPExpiry = _otpService.GenerateOTPExpiry();
            user.Password = HashPassword(password);
            user.IsEmailVerified = false;

            _context.Users.Add(user);
            _context.SaveChanges();

            try
            {
                _emailService.SendOTPEmail(user.Email, user.OTP);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email göndərilmədi: {ex.Message}");
            }

            return user;
        }

        public User Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && !u.IsDeleted);

            if (user == null || !VerifyPassword(password, user.Password))
                throw new Exception("Yanlış istifadəçi adı və ya şifrə");

            if (!user.IsEmailVerified)
                throw new Exception("Email təsdiqlənməyib. Zəhmət olmasa emailinizi yoxlayın.");

            return user;
        }

        public bool VerifyEmail(string email, string otpCode)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);

            if (user == null || !_otpService.ValidateOTP(user.OTP, otpCode, user.OTPExpiry))
                return false;

            user.IsEmailVerified = true;
            user.OTP = null;
            user.OTPExpiry = null;
            _context.SaveChanges();

            try
            {
                _emailService.SendWelcomeEmail(user.Email, $"{user.Name} {user.Surname}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Welcome email göndərilmədi: {ex.Message}");
            }

            return true;
        }

        public string GenerateOTP(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
            if (user == null)
                return null;

            user.OTP = _otpService.GenerateOTP();
            user.OTPExpiry = _otpService.GenerateOTPExpiry();
            _context.SaveChanges();

            try
            {
                _emailService.SendOTPEmail(user.Email, user.OTP);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email göndərilmədi: {ex.Message}");
            }

            return user.OTP;
        }

        public bool ForgotPassword(string email)
        {
            var otp = GenerateOTP(email);
            return otp != null;
        }

        public bool ResetPassword(string email, string newPassword, string otpCode)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
            if (user == null || !_otpService.ValidateOTP(user.OTP, otpCode, user.OTPExpiry))
                return false;

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
                throw new Exception("Şifrə ən az 6 simvol olmalıdır");

            user.Password = HashPassword(newPassword);
            user.OTP = null;
            user.OTPExpiry = null;
            _context.SaveChanges();

            return true;
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = _context.Users.Find(userId);
            if (user == null || user.IsDeleted || !VerifyPassword(currentPassword, user.Password))
                return false;

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
                throw new Exception("Yeni şifrə ən az 6 simvol olmalıdır");

            user.Password = HashPassword(newPassword);
            user.UpdatedDate = DateTime.Now;
            _context.SaveChanges();

            return true;
        }

        public bool ResendVerificationEmail(string email)
        {
            var otp = GenerateOTP(email);
            return otp != null;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        public List<User> GetUnverifiedUsers()
        {
            return _context.Users
                .Where(u => !u.IsEmailVerified && !u.IsDeleted)
                .ToList();
        }

        public List<User> GetUsersRegisteredBetween(DateTime startDate, DateTime endDate)
        {
            return _context.Users
                .Where(u => u.CreatedDate >= startDate && u.CreatedDate <= endDate && !u.IsDeleted)
                .ToList();
        }
    }
}