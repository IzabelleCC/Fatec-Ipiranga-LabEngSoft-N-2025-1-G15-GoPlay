using GoPlay_UserManagementService_Core.Entities;

namespace GoPlay_UserManagementService_Core.Business.Interfaces
{
    public interface IUserBusiness
    {
        /// <summary>
        /// Adiciona Usuário
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Add(UserEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Atualiza Usuário
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Update(UserEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Deleta Usuário
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Delete (string userName, CancellationToken cancellationToken);

        /// <summary>
        /// Busca Usuário por UserName
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserEntity> GetByUserName (string userName, CancellationToken cancellationToken);

    }
}
