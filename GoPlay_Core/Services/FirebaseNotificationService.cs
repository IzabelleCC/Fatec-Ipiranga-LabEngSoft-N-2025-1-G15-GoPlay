using Google.Apis.Auth.OAuth2;
using Google.Apis.FirebaseCloudMessaging.v1;
using Google.Apis.Services;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GoPlay_Core.Services
{
    public class FirebaseNotificationService
    {
        private readonly FirebaseCloudMessagingService _fcmService;
        private readonly string _projectId;
        private readonly ILogger<FirebaseNotificationService> _logger;
        private readonly IUserRepository _userRepository;

        public FirebaseNotificationService(IConfiguration configuration, ILogger<FirebaseNotificationService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _logger.LogInformation("Inicializando FirebaseNotificationService...");

            var base64String = configuration["FIREBASE_SERVICE_ACCOUNT_BASE64"];
            if (string.IsNullOrWhiteSpace(base64String))
            {
                _logger.LogError("Firebase Service Account Base64 não encontrado na configuração.");
                throw new InvalidOperationException("Firebase Service Account Base64 não encontrado na configuração.");
            }

            _logger.LogInformation("Decodificando credenciais da conta de serviço...");
            var jsonString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            var credential = GoogleCredential
                .FromJson(jsonString)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            _projectId = (credential.UnderlyingCredential as ServiceAccountCredential)?.ProjectId;
            if (string.IsNullOrWhiteSpace(_projectId))
            {
                _logger.LogError("ProjectId não encontrado na credencial.");
                throw new InvalidOperationException("ProjectId não encontrado na credencial.");
            }

            _logger.LogInformation("FirebaseNotificationService inicializado com ProjectId: {ProjectId}", _projectId);

            _fcmService = new FirebaseCloudMessagingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoPlayApp"
            });
            _userRepository = userRepository;
        }

        public async Task RegisterToken(string token, string userId) 
        {
            _logger.LogInformation("Registrando token {Token} para o usuário {UserId}", token, userId);
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("Token ou UserId inválidos. Token: {Token}, UserId: {UserId}", token, userId);
                throw new ArgumentException("Token e UserId são obrigatórios.");
            }
            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                _logger.LogError("Usuário com ID {UserId} não encontrado.", userId);
                throw new InvalidOperationException($"Usuário com ID {userId} não encontrado.");
            }
            user.FCMToken = token;
            await _userRepository.Update(user);
            _logger.LogInformation("Token {Token} registrado com sucesso para o usuário {UserId}", token, userId);
        }

        public async Task SendNotificationAsync(string fcmToken, string title, string body)
        {
            _logger.LogInformation("Preparando envio de notificação...");
            _logger.LogInformation("Token: {Token}", fcmToken);
            _logger.LogInformation("Título: {Title}, Corpo: {Body}", title, body);

            var message = new Google.Apis.FirebaseCloudMessaging.v1.Data.Message
            {
                Token = fcmToken,
                Notification = new Google.Apis.FirebaseCloudMessaging.v1.Data.Notification
                {
                    Title = title,
                    Body = body
                }
            };

            var request = new Google.Apis.FirebaseCloudMessaging.v1.Data.SendMessageRequest
            {
                Message = message
            };

            try
            {
                var response = await _fcmService.Projects.Messages.Send(request, $"projects/{_projectId}").ExecuteAsync();
                _logger.LogInformation("Notificação enviada com sucesso. Nome do response: {ResponseName}", response.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar notificação para o token: {Token}", fcmToken);
                throw;
            }
        }
    }
}
