namespace HospitalApp.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string OTP { get; set; }
        public DateTime? OTPExpiry { get; set; }
        public bool IsEmailVerified { get; set; } = false;

        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}