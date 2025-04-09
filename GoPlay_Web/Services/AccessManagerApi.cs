using System.Net.Http;

namespace GoPlay_Web.Services
{
    public class AccessManagerApi
    {
        private readonly HttpClient _httpClient;

        public AccessManagerApi(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("API");
        }



    }
}
