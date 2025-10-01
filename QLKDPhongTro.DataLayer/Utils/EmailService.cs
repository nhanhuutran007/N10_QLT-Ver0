using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace QLKDPhongTro.Presentation.Utils
{
    public static class EmailService
    {
        /// <summary>
        /// Gửi email bất đồng bộ qua Gmail SMTP
        /// </summary>
        public static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient("smtp.gmail.com", 587)) // Gmail SMTP
            {
                client.Credentials = new NetworkCredential("ngochai1521@gmail.com", "lskcvezrfmoaqiwb"); // ⚠️ Không nên hard-code
                client.EnableSsl = true;

                var mailMessage = new MailMessage("ngochai1521@gmail.com", toEmail, subject, body)
                {
                    IsBodyHtml = false // có thể chuyển sang true nếu muốn gửi HTML
                };

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
