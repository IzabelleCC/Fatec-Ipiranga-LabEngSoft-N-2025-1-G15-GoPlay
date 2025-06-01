using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace GoPlay_Core.Utils
{
    public static class CertificateLoader
    {
        public static X509Certificate2 LoadFromBase64(IConfiguration configuration)
        {
            var base64 = configuration["Gerencianet:CertificateBase64"];
            var password = configuration["Gerencianet:CertificatePassword"];

            if (string.IsNullOrWhiteSpace(base64))
                throw new InvalidOperationException("Certificado não encontrado nas configurações.");

            var rawData = Convert.FromBase64String(base64);

            return string.IsNullOrWhiteSpace(password)
                ? new X509Certificate2(rawData)
                : new X509Certificate2(rawData, password, X509KeyStorageFlags.MachineKeySet);
        }
    }
}
