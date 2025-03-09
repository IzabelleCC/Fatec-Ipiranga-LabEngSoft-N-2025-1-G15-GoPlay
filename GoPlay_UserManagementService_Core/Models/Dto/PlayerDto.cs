using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_UserManagementService_Core.Entities;

namespace GoPlay_UserManagementService_Core.Models.Dto
{
    public abstract class PlayerDto : UserDto
    {
        public override int IdUser { get; set; }
        public override string Name { get; set; }
        public override string Email { get; set; }
        public override string Login { get; set; }
        public override string Password { get; set; }
        public override string? InstagramPage { get; set; }
        public override int UserTypeId { get; set; }
        public string Cpf { get; set; }
        public string? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? TShirtSize { get; set; }

        protected PlayerDto()
        {
            
        }

        public PlayerDto(UserEntity entity) : base(entity)
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
            
            if(entity.Player != null)
            {
                Cpf = entity.Player.Cpf;
                Gender = entity.Player.Gender;
                BirthDate = entity.Player.BirthDate;
                TShirtSize = entity.Player.TShirtSize;
            }

        }
    }
}
