using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GoPlay_Core.Services
{
    public class EmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            using (SmtpClient smtpClient = new SmtpClient()) 
            {
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new System.Net.NetworkCredential("goplay.fatec@gmail.com", "mtzq xofu eanx fcno");

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("goplay.fatec@gmail.com", "GoPlay");
                    mailMessage.To.Add(email);
                    mailMessage.Subject = subject;
                    mailMessage.Body = message;

                    try
                    {
                        smtpClient.Send(mailMessage);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Erro ao enviar e-mail de confirmação de cadastro.", ex);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
