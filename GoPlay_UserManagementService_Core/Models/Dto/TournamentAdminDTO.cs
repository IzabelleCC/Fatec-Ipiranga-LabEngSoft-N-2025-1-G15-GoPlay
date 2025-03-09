using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_UserManagementService_Core.Entities;

namespace GoPlay_UserManagementService_Core.Models.Dto
{
    public abstract class TournamentAdminDto : UserDto
    {
        public override int IdUser { get; set; }
        public override string Name { get; set; }
        public override string Email { get; set; }
        public override string Login { get; set; }
        public override string Password { get; set; }
        public override string? InstagramPage { get; set; }
        public override int UserTypeId { get; set; }
        public string Cnpj {  get; set; }

        protected TournamentAdminDto()
        {

        }

        public TournamentAdminDto(UserEntity entity) : base(entity)
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

            if(entity.TournamentAdmin != null)
            {
                Cnpj = entity.TournamentAdmin.Cnpj;
            }

        }

    }
}
