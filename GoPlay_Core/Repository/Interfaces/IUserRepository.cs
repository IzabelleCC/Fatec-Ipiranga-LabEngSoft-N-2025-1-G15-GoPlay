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
        Task<UserEntity?> GetByEmailAndUserType(string email, int userType);
        Task<UserEntity?> GetByCpfCnpjAndUserType(string cpfCnpj, int userType);
        Task<bool> UpDatePassword(string userName, string password);
        Task<List<UserEntity?>> GetByName(string name);
    }
}
