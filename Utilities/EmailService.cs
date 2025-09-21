using System.Net;
using System.Net.Mail;

namespace HospitalApp.Utilities
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly bool _enableSsl;

        public EmailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, bool enableSsl = true)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _enableSsl = enableSsl;
        }

        public bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = _enableSsl;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_smtpUsername),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    client.Send(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email göndərilmədi: {ex.Message}");
                return false;
            }
        }

        public bool SendOTPEmail(string toEmail, string otpCode)
        {
            string subject = "HospitalApp - OTP Kodu";
            string body = $@"
                <h3>HospitalApp Təsdiq Kodu</h3>
                <p>Sizin OTP kodunuz: <strong>{otpCode}</strong></p>
                <p>Bu kod 30 dəqiqə ərzində etibarlıdır.</p>
                <br>
                <p>Əgər siz bu hesabı yaratmadınızsa, bu mesajı ignore edin.</p>";

            return SendEmail(toEmail, subject, body);
        }

        public bool SendPasswordResetEmail(string toEmail, string otpCode)
        {
            string subject = "HospitalApp - Şifrə Yeniləmə";
            string body = $@"
                <h3>Şifrə Yeniləmə Təsdiq Kodu</h3>
                <p>Sizin OTP kodunuz: <strong>{otpCode}</strong></p>
                <p>Bu kodla şifrənizi yeniləyə bilərsiniz.</p>
                <br>
                <p>Əgər siz şifrə yeniləmə istəməmisinizsə, hesabınızı yoxlayın.</p>";

            return SendEmail(toEmail, subject, body);
        }

        public bool SendWelcomeEmail(string toEmail, string fullName)
        {
            string subject = "HospitalApp - Xoş Gəlmisiniz!";
            string body = $@"
                <h3>Xoş Gəlmisiniz, {fullName}!</h3>
                <p>HospitalApp hesabınız uğurla yaradıldı.</p>
                <p>Artıq xəstəxana sistemimizdən istifadə edə bilərsiniz.</p>
                <br>
                <p>Hörmətlə,<br>HospitalApp Komandası</p>";

            return SendEmail(toEmail, subject, body);
        }
    }
}