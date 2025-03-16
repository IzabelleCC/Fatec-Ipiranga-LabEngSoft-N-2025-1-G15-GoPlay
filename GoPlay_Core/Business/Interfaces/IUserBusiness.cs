using GoPlay_UserManagementService_Core.Entities;

namespace GoPlay_UserManagementService_Core.Business.Interfaces
{
    public interface IUserBusiness<T> where T : UserEntity
    {
        /// <summary>
        /// Adiciona Usuário
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Add(T entity, CancellationToken cancellationToken);

        /// <summary>
        /// Atualiza Usuário
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Update(T entity, CancellationToken cancellationToken);

        /// <summary>
        /// Deleta Usuário
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Delete (string userName, CancellationToken cancellationToken);

        /// <summary>
        /// Busca Usuário por UserName
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T> GetByUserName (string userName, CancellationToken cancellationToken);

    }
}
