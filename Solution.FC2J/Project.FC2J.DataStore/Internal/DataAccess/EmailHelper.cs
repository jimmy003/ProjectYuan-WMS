using System.Net.Mail;
using System.Threading.Tasks;
using Project.FC2J.Models.Email;

namespace Project.FC2J.DataStore.Internal.DataAccess
{
    internal static class EmailHelper
    {
        private static readonly string _host = "smtp.gmail.com";
        private static readonly int _port = 587;

        public static async Task Send(this EmailPayload emailPayload)
        {
            var _settings = DBSettings.GetDBSettingsInstance();

            var mail = new MailMessage();
            var smtpServer = new SmtpClient(_host);
            mail.From = new MailAddress(_settings.EmailUsername);
            mail.To.Add(emailPayload.To);
            mail.Subject = emailPayload.Subject;
            mail.Body = emailPayload.Body;
            mail.IsBodyHtml = true;

            //var attachment = new Attachment(emailPayload.Attachment);
            //mail.Attachments.Add(attachment);

            smtpServer.Port = _port;
            smtpServer.Credentials = new System.Net.NetworkCredential(_settings.EmailUsername, _settings.EmailPassword);
            smtpServer.EnableSsl = true;

            await smtpServer.SendMailAsync(mail);
        }
    }
}
