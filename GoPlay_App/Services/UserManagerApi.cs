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

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                var param = new Dictionary<string, string?>
                {
                    { "token", token },
                    { "email", email }
                };

                var baseUrl = _configuration["Backend:BaseUrl"];

                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    return false;
                }

                var confirmationLink = QueryHelpers.AddQueryString($"{baseUrl}/api/UserManager/emailConfirmation", param);

                var response = await _httpClient.GetAsync(confirmationLink);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
