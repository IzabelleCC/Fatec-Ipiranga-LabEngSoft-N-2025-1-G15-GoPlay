using GoPlay_Core.Entities;

namespace GoPlay_Core.Business.Interfaces
{
    public interface IGameMatchBusiness
    {
        Task<List<GameMatchEntity>> GenerateEliminationMatches(int categoryId, CancellationToken cancellationToken);
        Task<List<GameMatchEntity>> InsertEliminationResultsAndReturnWinners(GameMatchEntity results, CancellationToken cancellationToken);
    }
}
