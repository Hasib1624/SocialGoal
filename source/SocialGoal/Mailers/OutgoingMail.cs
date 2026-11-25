using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SocialGoal.Web.Mailers
{
    public class OutgoingMail : MailMessage
    {
        private readonly IConfiguration mail;
        private readonly string contentRoot;
        private readonly ILogger logger;

        public OutgoingMail()
        {
        }

        public OutgoingMail(IConfiguration mail, string contentRoot, ILogger logger)
        {
            this.mail = mail;
            this.contentRoot = contentRoot;
            this.logger = logger;
        }

        /// <summary>Sends through SMTP when Mail:Host is set, otherwise writes an .eml file to the pickup directory.</summary>
        public virtual void Send()
        {
            From = new MailAddress(mail["From"]);
            using var client = new SmtpClient();
            if (string.IsNullOrEmpty(mail["Host"]))
            {
                var directory = Path.GetFullPath(mail["PickupDirectory"], contentRoot);
                Directory.CreateDirectory(directory);
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = directory;
                logger.LogInformation("Mail '{Subject}' written to {Directory}", Subject, directory);
            }
            else
            {
                client.Host = mail["Host"];
                client.Port = mail.GetValue("Port", 587);
                client.EnableSsl = true;
                if (!string.IsNullOrEmpty(mail["UserName"]))
                    client.Credentials = new NetworkCredential(mail["UserName"], mail["Password"]);
            }
            client.Send(this);
        }
    }
}
