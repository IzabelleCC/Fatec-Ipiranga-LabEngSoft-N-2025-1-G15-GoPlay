using System;

namespace GoPlay_Core.Utils
{
    public static class ConnectionStringHelper
    {
        public static string FromEnvironmentVariable(string variableName = "DATABASE_URL")
        {
            var databaseUrl = Environment.GetEnvironmentVariable(variableName);

            if (string.IsNullOrWhiteSpace(databaseUrl))
                throw new InvalidOperationException($"A variável de ambiente '{variableName}' não foi encontrada.");

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
