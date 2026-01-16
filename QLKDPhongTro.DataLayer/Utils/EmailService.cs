using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;

namespace QLKDPhongTro.Presentation.Utils
{
    public static class EmailService
    {
        // Read configuration once and cache
        private static readonly string SmtpHost = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
        private static readonly int SmtpPort = int.TryParse(ConfigurationManager.AppSettings["SmtpPort"], out var port) ? port : 587;
        private static readonly bool SmtpEnableSsl = bool.TryParse(ConfigurationManager.AppSettings["SmtpEnableSsl"], out var ssl) ? ssl : true;
        private static readonly string SmtpEmail = ConfigurationManager.AppSettings["SmtpEmail"] ?? throw new ConfigurationErrorsException("SmtpEmail not configured in App.config");
        private static readonly string SmtpPassword = ConfigurationManager.AppSettings["SmtpPassword"] ?? throw new ConfigurationErrorsException("SmtpPassword not configured in App.config");

        /// <summary>
        /// Gửi email bất đồng bộ qua Gmail SMTP
        /// </summary>
        public static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(SmtpHost, SmtpPort))
            {
                client.Credentials = new NetworkCredential(SmtpEmail, SmtpPassword);
                client.EnableSsl = SmtpEnableSsl;

                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(SmtpEmail, "HomeStead System"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }

        /// <summary>
        /// Gửi email bất đồng bộ qua Gmail SMTP với file đính kèm
        /// </summary>
        public static async Task SendEmailWithAttachmentAsync(string toEmail, string subject, string body, string attachmentFilePath)
        {
            using (var client = new SmtpClient(SmtpHost, SmtpPort))
            {
                client.Credentials = new NetworkCredential(SmtpEmail, SmtpPassword);
                client.EnableSsl = SmtpEnableSsl;

                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(SmtpEmail, "HomeStead System"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                // Thêm file đính kèm nếu file tồn tại
                if (!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
                {
                    var attachment = new Attachment(attachmentFilePath);
                    mailMessage.Attachments.Add(attachment);
                }

                await client.SendMailAsync(mailMessage);
            }
        }

        /// <summary>
        /// Gửi email bất đồng bộ qua Gmail SMTP với nhiều file đính kèm
        /// </summary>
        public static async Task SendEmailWithAttachmentsAsync(string toEmail, string subject, string body, System.Collections.Generic.List<string> attachmentFilePaths)
        {
            using (var client = new SmtpClient(SmtpHost, SmtpPort))
            {
                client.Credentials = new NetworkCredential(SmtpEmail, SmtpPassword);
                client.EnableSsl = SmtpEnableSsl;

                var mailMessage = new MailMessage()
                {
                    From = new MailAddress(SmtpEmail, "HomeStead System"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                // Thêm tất cả file đính kèm
                if (attachmentFilePaths != null)
                {
                    foreach (var attachmentFilePath in attachmentFilePaths)
                    {
                        if (!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
                        {
                            var attachment = new Attachment(attachmentFilePath);
                            mailMessage.Attachments.Add(attachment);
                        }
                    }
                }

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
