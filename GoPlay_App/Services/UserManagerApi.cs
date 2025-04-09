using Microsoft.AspNetCore.WebUtilities;

namespace GoPlay_Web.Services
{
    public class UserManagerApi
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UserManagerApi(IHttpClientFactory factory, IConfiguration configuration)
        {
            _httpClient = factory.CreateClient("API");
            _configuration = configuration;
        }

        public async Task<bool> EmailConfirmation(string token, string email)
        {

            var param = new Dictionary<string, string?>
            {
                {"token", token },
                {"email", email }
            };

            var baseUrl = _configuration["Backend:BaseUrl"];
            var confirmationLink = QueryHelpers.AddQueryString($"{baseUrl}/emailConfirmation", param);

            var response = await _httpClient.GetAsync(confirmationLink);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

    }
}
