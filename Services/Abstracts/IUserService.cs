using HospitalApp.Entities;

namespace HospitalApp.Services.Abstracts
{
    public interface IUserService
    {
        User Add(User user);
        User Update(User user);
        void Delete(int id);
        User GetById(int id);
        List<User> GetAll();
        User GetByUsername(string username);
        User GetByEmail(string email);

        User Register(User user, string password);
        User Login(string username, string password);
        bool VerifyEmail(string email, string otpCode);
        string GenerateOTP(string email);
        bool ForgotPassword(string email);
        bool ResetPassword(string email, string newPassword, string otpCode);
        bool ChangePassword(int userId, string currentPassword, string newPassword);
        bool ResendVerificationEmail(string email);

        List<User> GetUnverifiedUsers();
        List<User> GetUsersRegisteredBetween(DateTime startDate, DateTime endDate);
    }
}