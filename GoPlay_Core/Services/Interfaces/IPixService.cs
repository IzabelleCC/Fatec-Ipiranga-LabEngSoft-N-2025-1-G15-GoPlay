using GoPlay_Core.Entities;

namespace GoPlay_Core.Services.Interfaces
{
    public interface IPixService
    {
        Task<string> GeneratePixAsync(PixRequestData pixRequestData, string txid);
    }

}
