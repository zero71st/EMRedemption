using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EMRedemption.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class dzEmailSernder : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            SmtpClient client = new SmtpClient("mail.dzcard.com");
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("redemption_thmobilloyaltyclub@dzcard.com", "abc123!@#");

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("redemption_thmobilloyaltyclub@dzcard.com");
            mail.To.Add(email);
            mail.Subject = subject;
            mail.Body = message;

            client.Send(mail);

            return Task.CompletedTask;
        }
    }
}
