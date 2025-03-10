using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Repository.Interfaces;

namespace GoPlay_UserManagementService_Infra.Repository
{
    public class UserRepository : IUserRepository
    {
        public Task Add(UserEntity entity)
        {
            throw new NotImplementedException();
        }
        public Task Delete(UserEntity entity)
        {
            throw new NotImplementedException();
        }
        public Task<UserEntity?> GetById(int id)
        {
            throw new NotImplementedException();
        }
        public Task Update(UserEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
