using System;
using Microsoft.Extensions.Configuration;

namespace GoPlay_Core.Utils
{
    public  class ConnectionStringHelper
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringHelper(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string FromEnvironmentVariable(string variableName = "DATABASE_URL")
        {
            var databaseUrl = Environment.GetEnvironmentVariable(variableName);

            if (string.IsNullOrWhiteSpace(databaseUrl))
            {
                return _configuration["ConnectionStrings:GoPlayDb"];
            }

            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            return $"Host={databaseUri.Host};" +
                   $"Port={databaseUri.Port};" +
                   $"Database={databaseUri.AbsolutePath.TrimStart('/')};" +
                   $"Username={userInfo[0]};" +
                   $"Password={userInfo[1]};" +
                   "SSL Mode=Require;Trust Server Certificate=true;";
        }
    }
}
