using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        public EmailSender() { }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // *********************************************************************
            // Open the “Mail” app.
            // Open the “Settings” menu.
            // Select “Accounts” and then select your Google Account.
            // Replace your password with the 16 - character password shown above.
            // Password for gmail ---> xdek hwlp bqcu lamj <--- Password for gmail
            // *********************************************************************


            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("wingchunkungfuninja@gmail.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            // send email
            using (var emailClient = new SmtpClient())
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("wingchunkungfuninja@gmail.com", "xdek hwlp bqcu lamj");
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);
            }

            return Task.CompletedTask;
        }
    }
}
