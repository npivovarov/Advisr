using Advisr.Domain.DbModels;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Advisr.Web.Helpers
{
    public class EmailNitificationHelper
    { 
        private EmailNitificationHelper()
        {
        }

        public static EmailNitificationHelper Create()
        {
            return new EmailNitificationHelper();
        }

        public async Task SendEmailResetPassword(ApplicationUser user, ApplicationUserManager userManager, string callbackUrl)
        {
            var htmlBody = GetHtmlBody("ResetPassword");

            htmlBody = htmlBody.Replace("{username}", user.FirstName + " " + user.LastName);
            htmlBody = htmlBody.Replace("{callbackurl}", callbackUrl);

            await userManager.SendEmailAsync(user.Id, "ADVISR: Reset Password", htmlBody);
        }

        public async Task SendConfirmationEmail(ApplicationUser user, ApplicationUserManager userManager, string callbackUrl)
        {
            var htmlBody = GetHtmlBody("ConfirmationEmail");
            
            htmlBody = htmlBody.Replace("{callbackurl}", callbackUrl);

            await userManager.SendEmailAsync(user.Id, "ADVISR: Please confirm your registration", htmlBody);
        }

        private string GetHtmlBody(string nameOfTemplate)
        {
            string bodyHtml = string.Empty;

            var path = HostingEnvironment.MapPath(string.Format(@"~/Emails/{0}.{1}.html", nameOfTemplate, CultureHelper.CurrentCulture.TwoLetterISOLanguageName));

            using (FileStream html = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (System.IO.StreamReader sr = new StreamReader(html))
                {
                    bodyHtml = sr.ReadToEnd();
                }
            }

            return bodyHtml;
        }


        public Task ConfigSendGridasync(IdentityMessage message)
        {
            return SendEmail(message.Destination, message.Subject, message.Body, message.Body);
        }

        public Task SendEmail(string destination, string subject, string body, string text)
        {
            return SendEmail( new List<string> { destination }, null, subject, body, text, null);
        }

        public Task SendEmail(IEnumerable<string> destinations, IEnumerable<string> ccDestinations, string subject, string body, string text, List<Attachment> attachments)
        {
            if (text == null)
            {
                text = body;
            }

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["emailAccountInfo"], ConfigurationManager.AppSettings["emailAccountDisplayName"]);

            foreach (var email in destinations)
            {
                msg.To.Add(new MailAddress(email));
            }
            if (ccDestinations != null)
            {
                foreach (var email in ccDestinations)
                {
                    msg.CC.Add(new MailAddress(email));
                }
            }
            msg.Subject = subject;
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html));
            msg.Body = body;
            msg.IsBodyHtml = true;

            if (attachments != null && attachments.Count > 0)
            {
                foreach (var attachment in attachments)
                {
                    msg.Attachments.Add(attachment);
                }
            }

            SmtpClient smtpClient = new SmtpClient(); //From Web Config
            smtpClient.SendCompleted += OnSmtpClientSendCompleted;

            //return Task.Delay(1);

            return smtpClient.SendMailAsync(msg);
        }


        private void OnSmtpClientSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            SmtpClient smtpClient = (SmtpClient)sender;
            smtpClient.Dispose();
        }
    }
}