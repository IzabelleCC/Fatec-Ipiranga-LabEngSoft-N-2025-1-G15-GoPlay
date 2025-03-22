using System;

namespace GoPlay_Infra
{
    public static class HerokuConnectionHelper
    {
        public static string ConvertDatabaseUrlToConnectionString(string databaseUrl)
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');

            return $"Host={uri.Host};Port={uri.Port};Username={userInfo[0]};Password={userInfo[1]};Database={uri.AbsolutePath.TrimStart('/')};SSL Mode=Require;Trust Server Certificate=true";
        }
    }
}
