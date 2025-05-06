namespace GoPlay_App.Api.Controllers.TournamentManager
{
    public interface ITournamentBusiness<T1>
    {
        /// <summary>
        /// Adiciona Torneio
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Add(T1 entity, CancellationToken cancellationToken);

        /// <summary>
        /// Atualiza Torneio
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Update(T1 entity, CancellationToken cancellationToken);

        /// <summary>
        /// Deleta Torneio
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Delete(T1 entity, CancellationToken cancellationToken);

        /// <summary>
        /// Busca Torneio por Nome
        /// </summary>
        /// <param name="tournamentName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T1>> GetAllByTournamentName(string tournamentName, CancellationToken cancellationToken);

        /// <summary>
        /// Busca todos os Torneios
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<T1>> GetAllTournaments(CancellationToken cancellationToken);

        /// <summary>
        /// Busca Torneio por Id
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<T1> GetTournamentById(int tournamentId, CancellationToken cancellationToken);

    }
}