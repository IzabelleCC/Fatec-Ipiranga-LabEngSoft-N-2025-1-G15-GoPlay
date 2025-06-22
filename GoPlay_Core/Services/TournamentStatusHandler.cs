using GoPlay_Core.Enum;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoPlay_Core.Services
{
    public class TournamentStatusHandler
    {
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ILogger<TournamentStatusHandler> _logger;

        public TournamentStatusHandler(ITournamentRepository tournamentRepository, ILogger<TournamentStatusHandler> logger)
        {
            _tournamentRepository = tournamentRepository ?? throw new ArgumentNullException(nameof(tournamentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task UpdateTournamentStatusesAsync()
        {
            try
            {
                var tournaments = await _tournamentRepository.GetAllTournaments();

                if (tournaments == null || tournaments.Count == 0)
                {
                    _logger.LogInformation("Nenhum torneio para atualizar status.");
                    return;
                }

                var today = DateTime.UtcNow.Date;
                Console.WriteLine($"Atualizando status dos torneios em {today}...");
                foreach (var tournament in tournaments)
                {
                    if (tournament == null || !tournament.IsActive)
                        continue;

                    var oldStatus = tournament.Status;
                    TournamentStatusEnum newStatus = oldStatus;

                    var registrationDeadlineDate = tournament.RegistrationDeadline.Date;
                    var gamesStartDate = tournament.GamesStartDate.Date;
                    var gamesEndDate = tournament.GamesEndDate.Date;

                    if (today < registrationDeadlineDate)
                    {
                        newStatus = TournamentStatusEnum.InscricaoAberta;
                    }
                    else if (today >= registrationDeadlineDate && today < gamesStartDate && oldStatus != TournamentStatusEnum.ChavesPublicadas)
                    {
                        newStatus = TournamentStatusEnum.InscricaoEncerrada;
                    }
                    else if (today >= gamesStartDate && today <= gamesEndDate)
                    {
                        newStatus = TournamentStatusEnum.EmAndamento;
                    }
                    else if (today > gamesEndDate)
                    {
                        newStatus = TournamentStatusEnum.Encerrado;
                    }

                    if (oldStatus != newStatus)
                    {
                        tournament.Status = newStatus;
                        await _tournamentRepository.Update(tournament);

                        _logger.LogInformation("Torneio {Id}: status atualizado de {OldStatus} para {NewStatus}.",
                            tournament.Id, oldStatus, newStatus);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status dos torneios.");
                throw new InvalidOperationException("Erro ao atualizar status dos torneios.", ex);
            }
        }
    }
}
