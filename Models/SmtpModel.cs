using System.Net.Mail;
using System.Net;

namespace FilmFlow.Models
{
    public class SmtpModel
    {
        public bool sendEmail(string email, string topic, string body)
        {
            var smtpClient = new SmtpClient("smtp.yandex.ru")
            {
                UseDefaultCredentials = false,
                Port = 587,
                Credentials = new NetworkCredential("Regis.youtube@yandex.ru", "kjmtpynsqztvrgsh"),
                EnableSsl = true,
                Timeout = 1000,
            };
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress("Regis.youtube@yandex.ru");
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
