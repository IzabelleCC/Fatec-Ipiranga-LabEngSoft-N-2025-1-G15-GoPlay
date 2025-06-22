using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GoPlay_Core.Services
{
    public class ExpoNotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExpoNotificationService> _logger;

        public ExpoNotificationService(HttpClient httpClient, ILogger<ExpoNotificationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task SendNotificationAsync(string expoPushToken, string title, string body)
        {
            var payload = new
            {
                to = expoPushToken,
                title = title,
                body = body
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _logger.LogInformation("Enviando notificação para Expo token: {Token}", expoPushToken);

            var response = await _httpClient.PostAsync("https://exp.host/--/api/v2/push/send", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Notificação enviada com sucesso para {Token}", expoPushToken);
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Erro ao enviar notificação. Status: {Status}, Conteúdo: {Content}",
                    response.StatusCode, responseContent);
                throw new InvalidOperationException($"Erro no envio da notificação: {responseContent}");
            }
        }
    }
}
