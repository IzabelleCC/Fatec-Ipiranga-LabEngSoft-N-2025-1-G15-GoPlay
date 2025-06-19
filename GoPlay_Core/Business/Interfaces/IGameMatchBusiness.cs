using GoPlay_Core.Entities;
using GoPlay_Core.Models.Dto;

namespace GoPlay_Core.Business.Interfaces
{
    public interface IGameMatchBusiness
    {
        Task<List<GameMatchEntity>> GenerateEliminationMatches(int categoryId, CancellationToken cancellationToken);
        Task<List<GameMatchEntity>> InsertEliminationResultsAndReturnWinners(GameMatchEntity results, CancellationToken cancellationToken);
        Task<List<EliminationGameDto>> GetEliminationGamesByCategory(int categoryId, int matchStage, CancellationToken cancellationToken);
        Task InsertCourtNumberElimination(int categoryId, int numberGame, int courtNumber, CancellationToken cancellationToken);
    }
}
