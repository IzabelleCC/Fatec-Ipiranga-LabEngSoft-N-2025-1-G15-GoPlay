using GoPlay_Core.Entities;

namespace GoPlay_Core.Business.Interfaces
{
    public interface IPixBusiness
    {
        Task<string> GeneratePixForRegistration(int registrationId, string userId, CancellationToken cancellationToken);
        Task ConfirmPaymentByTxIdAsync(string txid, string userId, CancellationToken cancellationToken);
        Task<PixRequestData> CreatePixRequestData(int registrationId, string userId, CancellationToken cancellationToken);
        //Task RegisterWebhookAsync(string chavePix, string webhookUrl, CancellationToken cancellationToken);
    }
}
