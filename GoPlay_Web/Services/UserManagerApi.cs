using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace GoPlay_Web.Services
{
    public class UserManagerApi
    {
        private readonly HttpClient _httpClient;

        public UserManagerApi(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("API");
        }

        public async Task<bool> EmailConfirmation(string token, string email)
        {

            var param = new Dictionary<string, string?>
            {
                {"token", token },
                {"email", email }
            };

            var confirmationLink = QueryHelpers.AddQueryString($"https://localhost:7276/api/UserManager/emailConfirmation", param);

            var response = await _httpClient.GetAsync(confirmationLink);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

    }
}
