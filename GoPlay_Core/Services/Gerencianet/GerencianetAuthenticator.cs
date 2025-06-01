using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using GoPlay_Core.Utils;

namespace GoPlay_Core.Services.Gerencianet
{
    public class GerencianetAuthenticator
    {
        private readonly IConfiguration _configuration;

        public GerencianetAuthenticator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(HttpClient client, string accessToken)> AuthenticateAsync()
        {
            var baseUrl = _configuration["Gerencianet:BaseUrl"];
            var clientId = _configuration["Gerencianet:ClientId"];
            var clientSecret = _configuration["Gerencianet:ClientSecret"];

            var cert = CertificateLoader.LoadFromBase64(_configuration);

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(cert);

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };

            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var requestBody = new StringContent(
                "grant_type=client_credentials&scope=cob.read cob.write pix.read pix.write webhook.read webhook.write",
                Encoding.UTF8, "application/x-www-form-urlencoded");


            var tokenResponse = await client.PostAsync("/oauth/token", requestBody);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorDetails = await tokenResponse.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Erro HTTP ao autenticar: {tokenResponse.StatusCode} - {errorDetails}");
            }

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonDocument.Parse(tokenJson).RootElement.GetProperty("access_token").GetString();

            return (client, token);
        }
    }
}
