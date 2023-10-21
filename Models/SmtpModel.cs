using System.Net.Mail;
using System.Net;

namespace FilmFlow.Models
{
    public class SmtpModel
    {
        public bool sendEmail(string email, string topic, string body)
        {
            var smtpClient = new SmtpClient(FilmFlow.Properties.Settings.Default.smtpServer)
            {
                UseDefaultCredentials = false,
                Port = 587,
                Credentials = new NetworkCredential(FilmFlow.Properties.Settings.Default.smtpUser, FilmFlow.Properties.Settings.Default.smtpPassword),
                EnableSsl = true,
                Timeout = 1000,
            };
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress(FilmFlow.Properties.Settings.Default.smtpMailFrom);
            message.Subject = topic;
            message.Body = body;
            try
            {
                smtpClient.Send(message);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
