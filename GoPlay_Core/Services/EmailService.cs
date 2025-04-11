using GoPlay_Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using GoPlay_Core.Services.Interfaces;

namespace GoPlay_Core.Services
{
    public class EmailService : IEmailService
    {

        public readonly EmailSender _emailSender;
        public readonly UserManager<UserEntity> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public EmailService(EmailSender emailSender, UserManager<UserEntity> userManager, IConfiguration configuration, ITokenService tokenService)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
            _tokenService = tokenService;
        }
        public async Task SendEmailRegisterAsync(UserEntity user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string?>
            {
                {"token", token },
                {"email", user.Email }
            };

            var baseUrl = _configuration["Backend:BaseUrl"];
            var confirmationLink = QueryHelpers.AddQueryString($"{baseUrl}/EmailConfirmation", param);

            string subject = "Confirmação de Cadastro";
            string message = $@"
                                    <p>Olá <strong>{user.Name}</strong>, seja bem-vindo ao GoPlay!</p>
                                    <p>Para confirmar seu cadastro, clique no link abaixo:</p>
                                    <p><a href=""{confirmationLink}"">{confirmationLink}</a></p>";

            try
            {
                await _emailSender.SendEmailAsync(user.Email ?? string.Empty, subject, message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao enviar e-mail de confirmação de cadastro.", ex);
            }
        }

        public async Task SendPasswordResetLinkAsync(UserEntity user)
        {
            var token = await _tokenService.GenerateToken(user);
            var param = new Dictionary<string, string?> { { "token", token } };

            var appBaseUrl = _configuration["Backend:BaseUrl"];
            var resetLink = QueryHelpers.AddQueryString($"{appBaseUrl}/ResetPassword", param);

            string subject = "Redefinição de Senha";
            string message = $@"
                                    <p>Olá <strong>{user.Name}</strong>,</p>
                                    <p>Para redefinir sua senha, clique no link abaixo:</p>
                                    <p><a href=""{resetLink}"">{resetLink}</a></p>";
            try
            {
                await _emailSender.SendEmailAsync(user.Email ?? string.Empty, subject, message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao enviar e-mail de confirmação de cadastro.", ex);
            }
        }
    }
}
