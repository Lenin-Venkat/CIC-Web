using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CICLatest.Models
{
    public class Email
    {
        private readonly EmailConfiguration _emailConfig;
        static string emailserver = "";

        public Email(EmailConfiguration email)
        {
            _emailConfig = email;
            emailserver = _emailConfig.SmtpServer;
        }
        public async Task SendAsync(string to, string subject, string body, byte[] attchment)
        {
            try
            {
                string toEmail = to;
                
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailConfig.UserName, "CIC")
                };
                mail.To.Add(new MailAddress(toEmail));
                //mail.CC.Add(new MailAddress("mvubunm@gmail.com"));

                mail.Attachments.Add(new Attachment(new MemoryStream(attchment), "Certificate.pdf"));

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(_emailConfig.SmtpServer, 25))
                {
                    smtp.Credentials = new NetworkCredential(_emailConfig.UserName, _emailConfig.Password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        public async Task SendPaymentAsync(string subject, string body, byte[] attchment,string mimeType)
        {
            try
            {
                string toEmail = "icmanager@gmail.com";
                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(_emailConfig.UserName, "CIC")
                };
                mail.To.Add(new MailAddress(toEmail));
                mail.CC.Add(new MailAddress("Support@cic.co.sz"));
                mail.CC.Add(new MailAddress("mvubunm@gmail.com"));
                ///mail.CC.Add(new MailAddress("surya@cloud4sa.co.za"));


                mail.Attachments.Add(new Attachment(new MemoryStream(attchment), "Payment."+ mimeType));

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(_emailConfig.SmtpServer, 25))
                {
                    smtp.Credentials = new NetworkCredential(_emailConfig.UserName, _emailConfig.Password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }
    }
}
