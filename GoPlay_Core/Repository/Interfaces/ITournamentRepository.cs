using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface ITournamentRepository
    {
        Task Add(TournamentEntity entity);
        Task Update(TournamentEntity entity);
        Task Delete(TournamentEntity entity);
        Task<List<TournamentEntity?>> GetAllTournaments();
        Task<TournamentEntity?> GetById(int id);
        Task<TournamentEntity?> GetByName(string name);
        Task<List<TournamentEntity?>> GetByLocation(string location);
        Task<List<TournamentEntity?>> GetByDate(DateTime date);
    }
}
