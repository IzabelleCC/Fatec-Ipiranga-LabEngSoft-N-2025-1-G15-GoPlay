using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_UserManagementService_Core.Entities;

namespace GoPlay_UserManagementService_Core.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task Add(UserEntity entity);
        Task Update(UserEntity entity, int id);
        Task Delete(UserEntity entity);
        Task<UserEntity?> GetById(int id);

    }
}
