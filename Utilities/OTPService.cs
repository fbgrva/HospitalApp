namespace HospitalApp.Utilities
{
    public class OTPService
    {
        public string GenerateOTP()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); 
        }

        public DateTime GenerateOTPExpiry(int minutes = 30)
        {
            return DateTime.Now.AddMinutes(minutes);
        }

        public bool ValidateOTP(string storedOTP, string inputOTP, DateTime? expiryTime)
        {
            if (string.IsNullOrEmpty(storedOTP) || string.IsNullOrEmpty(inputOTP))
                return false;

            if (expiryTime.HasValue && expiryTime.Value < DateTime.Now)
                return false;

            return storedOTP == inputOTP;
        }

        public bool IsOTPExpired(DateTime? expiryTime)
        {
            return expiryTime.HasValue && expiryTime.Value < DateTime.Now;
        }
    }
}