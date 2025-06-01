using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GoPlay_Infra.Repository
{
    public class CategoryPlayerRepository : ICategoryPlayerRepository
    {
        private readonly GoPlayDbContext _context;
        private readonly ILogger<CategoryPlayerRepository> _logger;

        public CategoryPlayerRepository(GoPlayDbContext context, ILogger<CategoryPlayerRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<CategoryPlayerEntity>> GetAllAsync()
        {
            try
            {
                return await _context.CategoryPlayers.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar todas as inscrições.");
                throw new InvalidOperationException("Erro ao recuperar todas as inscrições.", ex);
            }
        }

        public async Task<List<CategoryPlayerEntity>> GetByCategoryIdAsync(int categoryId)
        {
            try
            {
                return await _context.CategoryPlayers
                    .Where(cp => cp.CategoryId == categoryId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar inscrições por categoria.");
                throw new InvalidOperationException("Erro ao recuperar inscrições por categoria.", ex);
            }
        }

        public async Task<CategoryPlayerEntity?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.CategoryPlayers
                    .FirstOrDefaultAsync(cp => cp.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar inscrição por ID.");
                throw new InvalidOperationException("Erro ao recuperar inscrição por ID.", ex);
            }
        }

        public async Task<List<CategoryPlayerEntity>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _context.CategoryPlayers
                    .Where(cp => cp.FirstUserId == userId || cp.SecondUserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar inscrições por usuário.");
                throw new InvalidOperationException("Erro ao recuperar inscrições por usuário.", ex);
            }
        }

        public async Task AddAsync(CategoryPlayerEntity entity)
        {
            try
            {
                await _context.CategoryPlayers.AddAsync(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar a inscrição.");
                throw new InvalidOperationException("Erro ao registrar a inscrição.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var entity = await _context.CategoryPlayers
                    .FirstOrDefaultAsync(cp => cp.Id == id);

                if (entity == null)
                    throw new KeyNotFoundException("Inscrição não encontrada.");

                _context.CategoryPlayers.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir a inscrição.");
                throw new InvalidOperationException("Erro ao excluir a inscrição.", ex);
            }
        }

        public async Task UpdatePlayersAsync(CategoryPlayerEntity updatedEntity)
        {
            try
            {
                var existingEntity = await _context.CategoryPlayers
                    .FirstOrDefaultAsync(cp => cp.Id == updatedEntity.Id);

                if (existingEntity == null)
                    throw new KeyNotFoundException("Inscrição não encontrada.");

                existingEntity.FirstUserId = updatedEntity.FirstUserId;
                existingEntity.SecondUserId = updatedEntity.SecondUserId;
                existingEntity.FirstUserPaymentConfirmed = updatedEntity.FirstUserPaymentConfirmed;
                existingEntity.SecondUserPaymentConfirmed = updatedEntity.SecondUserPaymentConfirmed;
                existingEntity.RegisterStatus = updatedEntity.RegisterStatus;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar a inscrição.");
                throw new InvalidOperationException("Erro ao atualizar a inscrição.", ex);
            }
        }

        public async Task<CategoryPlayerEntity?> GetByTxIdAsync(string txid)
        {
            try
            {
                return await _context.CategoryPlayers
                    .FirstOrDefaultAsync(cp => cp.FirstUserTxId == txid || cp.SecondUserTxId == txid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar inscrição por TxId.");
                throw new InvalidOperationException("Erro ao buscar inscrição por TxId.", ex);
            }
        }
    }
}
