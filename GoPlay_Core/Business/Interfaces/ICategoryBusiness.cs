namespace GoPlay_Core.Business.Interfaces
{
    public interface ICategoryBusiness<T1>
    {
        Task Add(T1 entity, CancellationToken cancellationToken);
        Task Delete(T1 entity, CancellationToken cancellationToken);
        Task<List<T1?>> GetAllCategories(CancellationToken cancellationToken);
        Task<T1?> GetByCategoryType(string categoryType, CancellationToken cancellationToken);
        Task<T1?> GetById(int id, CancellationToken cancellationToken);
        Task<List<T1?>> GetByTournamentId(int tournamentId, CancellationToken cancellationToken);
        Task Update(T1 entity, CancellationToken cancellationToken);
        Task RegisterUserToCategory(int categoryId, string firstUserId, string? secongUserId, CancellationToken cancellationToken);
    }
}
