using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoPlay_Infra.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly GoPlayDbContext _context;
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(GoPlayDbContext context, ILogger<CategoryRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Add(CategoryEntity entity)
        {
            try
            {
                await _context.Categories.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar uma nova categoria.");
                throw new InvalidOperationException("Erro ao adicionar categoria.", ex);
            }
        }

        public async Task Update(CategoryEntity entity)
        {
            try
            {
                _context.Categories.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar a categoria.");
                throw new InvalidOperationException("Erro ao atualizar categoria.", ex);
            }
        }

        public async Task Delete(CategoryEntity entity)
        {
            try
            {
                _context.Categories.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir a categoria.");
                throw new InvalidOperationException("Erro ao excluir categoria.", ex);
            }
        }

        public async Task<List<CategoryEntity?>> GetAllCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Players)
                    .ToListAsync();

                if (categories == null || categories.Count == 0)
                {
                    _logger.LogWarning("Nenhuma categoria encontrada.");
                    return new List<CategoryEntity?>();
                }

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar as categorias.");
                throw new InvalidOperationException("Erro ao recuperar categorias.", ex);
            }
        }

        public async Task<CategoryEntity?> GetById(int id)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Players)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar a categoria por ID.");
                throw new InvalidOperationException("Erro ao recuperar categoria por ID.", ex);
            }
        }

        public async Task<CategoryEntity?> GetByCategoryType(string categoryType)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Players)
                    .FirstOrDefaultAsync(c => c.CategoryType.ToLower() == categoryType.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar a categoria pelo tipo.");
                throw new InvalidOperationException("Erro ao recuperar categoria pelo tipo.", ex);
            }
        }

        public async Task<List<CategoryEntity?>> GetByTournamentId(int tournamentId)
        {
            try
            {
                return await _context.Categories
                    .Include(c => c.Players)
                    .Where(c => c.TournamentId == tournamentId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar categorias pelo ID do torneio.");
                throw new InvalidOperationException("Erro ao recuperar categorias pelo ID do torneio.", ex);
            }
        }
    }
}
