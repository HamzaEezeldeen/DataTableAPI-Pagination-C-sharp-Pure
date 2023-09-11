using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace SportProductsWeb.Services
{
    public class AppEmailService : IEmailSender
    {
        private EmailSetting EmailSetting;

        public AppEmailService(EmailSetting emailSetting)
        {
            EmailSetting = emailSetting;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                MimeMessage msg = new MimeMessage();
                msg.From.Add(new MailboxAddress(EmailSetting.Sender, EmailSetting.Email));
                msg.To.Add(new MailboxAddress("", email));
                msg.Subject = subject;

                BodyBuilder msgBuilder = new BodyBuilder();
                msgBuilder.HtmlBody = htmlMessage;

                msg.Body = msgBuilder.ToMessageBody();

                SmtpClient smtpClient = new SmtpClient();

                if (EmailSetting.UseSSL)
                {
                    smtpClient.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 |
                                              System.Security.Authentication.SslProtocols.Ssl3
                                           | System.Security.Authentication.SslProtocols.Tls11;

                    await smtpClient.ConnectAsync(EmailSetting.Host, EmailSetting.Port, MailKit.Security.SecureSocketOptions.Auto);
                }
                else
                {
                    await smtpClient.ConnectAsync(EmailSetting.Host, EmailSetting.Port, MailKit.Security.SecureSocketOptions.None);
                }

                await smtpClient.AuthenticateAsync(EmailSetting.Email, EmailSetting.Password);
                await smtpClient.SendAsync(msg);

                smtpClient.Disconnect(true);
                smtpClient.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}