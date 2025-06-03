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
        Task AddRangeAsync(List<MatchGroupEntity> matches);
        Task<List<MatchGroupEntity>> GetByCategoryAsync(int categoryId);
        Task<MatchGroupEntity?> GetByIdAsync(int id);
        Task UpdateAsync(MatchGroupEntity match);
        Task DeleteAsync(MatchGroupEntity match);
    }
}
