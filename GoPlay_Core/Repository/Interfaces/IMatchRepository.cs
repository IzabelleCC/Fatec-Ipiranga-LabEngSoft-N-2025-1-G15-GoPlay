using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface IMatchRepository
    {
        Task AddRangeAsync(List<MatchEntity> matches);
        Task<List<MatchEntity>> GetByCategoryAsync(int categoryId);
        Task<MatchEntity?> GetByIdAsync(int id);
        Task UpdateAsync(MatchEntity match);
        Task DeleteAsync(MatchEntity match);
    }
}
