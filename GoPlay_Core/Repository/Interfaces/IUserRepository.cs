using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_Core.Entities;

namespace GoPlay_Core.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task Add(UserEntity entity);
        Task Update(UserEntity entity);
        Task Delete(UserEntity entity);
        Task<UserEntity?> GetById(string id);
        Task<UserEntity?> GetByUserName(string userName);
    }
}
