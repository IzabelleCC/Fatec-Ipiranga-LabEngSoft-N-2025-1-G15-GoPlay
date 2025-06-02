using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GoPlay_Core.Entities;
using GoPlay_Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GoPlay_Core.Services.Gerencianet
{
    public class PixService : IPixService
    {
        private readonly IConfiguration _configuration;
        private readonly GerencianetAuthenticator _authenticator;

        public PixService(IConfiguration configuration, GerencianetAuthenticator authenticator)
        {
            _configuration = configuration;
            _authenticator = authenticator;
        }

        public async Task<string> GeneratePixAsync(PixRequestData data, string txid)
        {
            try
            {
                var baseUrl = _configuration["Gerencianet:BaseUrl"];
                var pixKey = _configuration["Gerencianet:PixKey"];

                var (client, token) = await _authenticator.AuthenticateAsync();

                var json = JsonSerializer.Serialize(data);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.PutAsync($"{baseUrl}/v2/cob/{txid}", new StringContent(json, Encoding.UTF8, "application/json"));
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Serialize(new
                    {
                        error = $"Erro ao gerar cobrança Pix: {response.StatusCode}",
                        details = responseBody
                    });
                }

                //using var doc = JsonDocument.Parse(responseBody);
                //var qrCode = doc.RootElement.GetProperty("loc").GetProperty("location").GetString();
                //var brCode = doc.RootElement.GetProperty("pixCopiaECola").GetString();

                return JsonSerializer.Serialize(new
                {
                    message = "Cobrança Pix gerada com sucesso.",
                    responseBody,
                });
            }
            catch (HttpRequestException ex)
            {
                return JsonSerializer.Serialize(new { error = "Erro HTTP ao tentar gerar Pix", details = ex.Message });
            }
            catch (JsonException ex)
            {
                return JsonSerializer.Serialize(new { error = "Erro ao processar JSON da resposta da Gerencianet", details = ex.Message });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new { error = "Erro inesperado ao gerar Pix", details = ex.Message });
            }
        }

    }
}
