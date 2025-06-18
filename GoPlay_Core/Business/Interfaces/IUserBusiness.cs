using GoPlay_Core.Entities;
using Microsoft.AspNetCore.Http;

namespace GoPlay_Core.Business.Interfaces
{
    public interface IUserBusiness<TUserEntity, TUserResponse>
                                            where TUserEntity : class
                                            where TUserResponse : class
    {
        /// <summary>
        /// Adiciona Usuário
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Add(TUserEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Atualiza Usuário
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Update(TUserEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Deleta Usuário
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Delete(string userName, CancellationToken cancellationToken);

        /// <summary>
        /// Busca Usuário por UserName
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TUserEntity> GetByUserName(string userName, CancellationToken cancellationToken);

        /// <summary>
        /// Busca todos os jogadores
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TUserResponse>> GetAllPlayers(CancellationToken cancellationToken);

        Task<List<TUserResponse>> GetByName(string name, CancellationToken cancellationToken);

        Task<string?> UploadProfilePictureAsync(string userId, IFormFile file);

    }
}
