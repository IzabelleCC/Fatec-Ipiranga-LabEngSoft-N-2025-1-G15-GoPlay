using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_UserManagementService_Core.Entities;

namespace GoPlay_UserManagementService_Core.Models.Dto
{
    public abstract class UserDto
    {
        public virtual int IdUser { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string Login { get; set; }
        public virtual string Password { get; set; }
        public virtual string? InstagramPage { get; set; }
        public virtual int UserTypeId { get; set; }
        public virtual UserTypeEntity UserType { get; set; }
        public virtual PlayerEntity? Player { get; set; }
        public virtual TournamentAdminEntity? TournamentAdmin { get; set; }

        protected UserDto()
        {
            
        }
        protected UserDto(UserEntity entity)
        {
            IdUser = entity.IdUser;
            Name = entity.Name;
            Email = entity.Email;
            Login = entity.Login;
            Password = entity.Password;
            InstagramPage = entity.InstagramPage;
            UserTypeId = entity.UserTypeId;
            UserType = entity.UserType;
            Player = entity.Player;
            TournamentAdmin = entity.TournamentAdmin;
        }
    }
}
