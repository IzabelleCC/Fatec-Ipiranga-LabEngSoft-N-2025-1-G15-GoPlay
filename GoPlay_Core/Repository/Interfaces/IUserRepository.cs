using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task Add(UserEntity entity);
        Task Update(UserEntity entity);
        Task Delete(UserEntity entity);
        Task<List<UserEntity?>> GetAllPlayers();
        Task<UserEntity?> GetById(string id);
        Task<UserEntity?> GetByUserName(string userName);
        Task<UserEntity?> GetByEmail(string email);
        Task<UserEntity?> GetByCpfCnpj(string cpfCnpj);
        Task<bool> UpDatePassword(string userName, string password);

    }
}
