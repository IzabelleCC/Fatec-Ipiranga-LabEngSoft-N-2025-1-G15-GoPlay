using Google.Apis.Auth.OAuth2;
using Google.Apis.FirebaseCloudMessaging.v1;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;

namespace GoPlay_Core.Services
{
    public class FirebaseNotificationService
    {
        private readonly FirebaseCloudMessagingService _fcmService;
        private readonly string _projectId;

        public FirebaseNotificationService(IConfiguration configuration)
        {
            var base64String = configuration["FIREBASE_SERVICE_ACCOUNT_BASE64"];
            if (string.IsNullOrWhiteSpace(base64String))
            {
                throw new InvalidOperationException("Firebase Service Account Base64 não encontrado na configuração.");
            }

            var jsonString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            var credential = GoogleCredential
                .FromJson(jsonString)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            _projectId = (credential.UnderlyingCredential as ServiceAccountCredential)?.ProjectId
                         ?? throw new InvalidOperationException("ProjectId não encontrado na credencial.");

            _fcmService = new FirebaseCloudMessagingService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoPlayApp"
            });
        }

        public async Task SendNotificationAsync(string fcmToken, string title, string body)
        {
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

            Console.WriteLine($"Enviando notificação para o token: {fcmToken}");

            var response = await _fcmService.Projects.Messages.Send(request, $"projects/{_projectId}").ExecuteAsync();
            Console.WriteLine($"Notificação enviada: {response.Name}");
        }
    }
}
