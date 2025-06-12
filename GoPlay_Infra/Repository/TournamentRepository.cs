using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoPlay_Infra.Repository
{
    public class TournamentRepository : ITournamentRepository
    {
        private readonly GoPlayDbContext _context;
        private readonly ILogger<TournamentRepository> _logger;

        public TournamentRepository(GoPlayDbContext context, ILogger<TournamentRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Add(TournamentEntity entity)
        {
            try
            {
                await _context.Tournaments.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar novo torneio.");
                throw new InvalidOperationException("Erro ao adicionar torneio.", ex);
            }
        }

        public async Task Update(TournamentEntity entity)
        {
            try
            {
                var existingTournament = await _context.Tournaments
                    .Include(t => t.Categories)
                    .FirstOrDefaultAsync(t => t.Id == entity.Id);

                if (existingTournament != null)
                {
                    // Atualizar propriedades do torneio
                    _context.Entry(existingTournament).CurrentValues.SetValues(entity);

                    // Remover categorias que não estão mais na lista
                    var categoriasParaRemover = existingTournament.Categories
                        .Where(category => !entity.Categories.Any(c => c.Id == category.Id))
                        .ToList();

                    foreach (var category in categoriasParaRemover)
                    {
                        existingTournament.Categories.Remove(category);
                        _context.Categories.Remove(category);
                    }

                    // Adicionar novas categorias
                    foreach (var category in entity.Categories)
                    {
                        if (!existingTournament.Categories.Any(c => c.Id == category.Id))
                        {
                            existingTournament.Categories.Add(category);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar torneio.");
                throw new InvalidOperationException("Erro ao atualizar torneio.", ex);
            }
        }

        public async Task Delete(TournamentEntity entity)
        {
            try
            {
                entity.IsActive = false;
                _context.Tournaments.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar torneio.");
                throw new InvalidOperationException("Erro ao desativar torneio.", ex);
            }
        }

        public async Task<List<TournamentEntity?>> GetAllTournaments()
        {
            try
            {
                var tournaments = await _context.Tournaments
                    .Include(t => t.Categories)
                        .ThenInclude(c => c.CategoryPlayers)
                    .ToListAsync();

                if (tournaments == null || tournaments.Count == 0)
                {
                    _logger.LogWarning("Nenhum torneio encontrado.");
                    return new List<TournamentEntity?>();
                }

                return tournaments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar torneios.");
                throw new InvalidOperationException("Erro ao recuperar torneios.", ex);
            }
        }

        public async Task<List<TournamentEntity?>> GetTournamentByAdmUserId(string id)
        {
            try
            {
                var tournaments = await _context.Tournaments
                    .Include(t => t.Categories)
                    .Where(t => t.AdmUserId == id)
                    .ToListAsync();

                if (tournaments.Count == 0)
                {
                    _logger.LogWarning("Nenhum torneio encontrado para o ID do administrador: {Id}", id);
                }

                return tournaments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar torneios por ID do administrador.");
                throw new InvalidOperationException("Erro ao recuperar torneios por ID do administrador.", ex);
            }
        }
        public async Task<TournamentEntity?> GetById(int id)
        {
            try
            {
                var tournament = await _context.Tournaments
                    .Include(t => t.Categories)
                    .ThenInclude(c => c.CategoryPlayers)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (tournament == null)
                {
                    _logger.LogWarning("Torneio não encontrado com ID: {Id}", id);
                    throw new InvalidOperationException($"Torneio não encontrado para o ID {id}.");
                }


                return tournament;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar torneio por ID.");
                throw new InvalidOperationException("Erro ao recuperar torneio por ID.", ex);
            }
        }

        public async Task<TournamentEntity?> GetByName(string name)
        {
            try
            {
                var tournament = await _context.Tournaments
                    .Include(t => t.Categories)
                    .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

                return tournament;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar torneio por nome.");
                throw new InvalidOperationException("Erro ao recuperar torneio por nome.", ex);
            }
        }

        public async Task<List<TournamentEntity?>> GetAllByName(string name)
        {
            try
            {
                var tournament = await _context.Tournaments
                    .Include(t => t.Categories)
                    .Where(t => t.Name.ToLower().Contains(name.ToLower()))
                    .ToListAsync();

                return tournament;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar torneio por nome.");
                throw new InvalidOperationException("Erro ao recuperar torneio por nome.", ex);
            }
        }

        public async Task<List<TournamentEntity?>> GetByLocation(string location)
        {
            try
            {
                var tournaments = await _context.Tournaments
                    .Include(t => t.Categories)
                    .Where(t => t.Location.ToLower().Contains(location.ToLower()))
                    .ToListAsync();

                if (tournaments == null || tournaments.Count == 0)
                {
                    _logger.LogWarning("Nenhum torneio encontrado para o local: {Location}", location);
                    return new List<TournamentEntity?>();
                }

                return tournaments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar torneios por local.");
                throw new InvalidOperationException("Erro ao recuperar torneios por local.", ex);
            }
        }

        public async Task<List<TournamentEntity?>> GetByDate(DateTime date)
        {
            try
            {
                var tournaments = await _context.Tournaments
                    .Include(t => t.Categories)
                    .Where(t => t.GamesStartDate.Date == date.Date)
                    .ToListAsync();

                if (tournaments == null || tournaments.Count == 0)
                {
                    _logger.LogWarning("Nenhum torneio encontrado para a data: {Date}", date.ToShortDateString());
                    return new List<TournamentEntity?>();
                }

                return tournaments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar torneios por data.");
                throw new InvalidOperationException("Erro ao recuperar torneios por data.", ex);
            }
        }
    }
}
