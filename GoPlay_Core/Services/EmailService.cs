using GoPlay_Core.Services;
using GoPlay_Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace GoPlay_Core.Services
{
    public class EmailService : IEmailSender<UserEntity>
    {

        public readonly EmailSender _emailSender;
        public readonly UserManager<UserEntity> _userManager;
        private readonly IConfiguration _configuration;

        public EmailService(EmailSender emailSender, UserManager<UserEntity> userManager, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task SendEmailRegisterAsync(UserEntity user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string?>
            {
                {"token", token },
                {"email", user.Email }
            };

            var baseUrl = _configuration["Frontend:BaseUrl"];
            var confirmationLink = QueryHelpers.AddQueryString($"{baseUrl}/api/UserManager/emailConfirmation", param);

            string subject = "Confirmação de Cadastro";
            string message = $"Olá {user.Name}, seja bem-vindo ao GoPlay! Para confirmar seu cadastro, clique no link a seguir: {confirmationLink}";

            try
            {
                await _emailSender.SendEmailAsync(user.Email ?? string.Empty, subject, message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao enviar e-mail de confirmação de cadastro.", ex);
            }
        }

        public Task SendConfirmationLinkAsync(UserEntity user, string email, string confirmationLink)
        {
            return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(UserEntity user, string email, string resetLink)
        {
            try
            {
                string subject = "Redefinição de Senha";
                string message = $"Olá {user.Name}, para redefinir sua senha, clique no link a seguir:\r\n {resetLink}";

               return  _emailSender.SendEmailAsync(user.Email ?? string.Empty, subject, message);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao enviar e-mail de redefinição de senha.", ex);
            }
        }

        public Task SendPasswordResetCodeAsync(UserEntity user, string email, string resetCode)
        {
            return _userManager.ChangePasswordAsync(user, email, resetCode);
        }
    }
}
