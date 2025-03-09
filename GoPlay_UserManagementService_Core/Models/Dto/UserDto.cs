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
        public virtual int UserType { get; set; }
        public virtual string? InstagramPage { get; set; }
        public virtual string? Cpf { get; set; }
        public virtual string? Cnpj { get; set; }
        public virtual string? Gender { get; set; }
        public virtual DateOnly? BirthDate { get; set; }
        public virtual string? TShirtSize { get; set; }



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
            UserType = (int)entity.UserType;
            InstagramPage = entity.InstagramPage ?? string.Empty;
            Cpf = entity.Cpf ?? string.Empty;
            Cnpj = entity.Cnpj ?? string.Empty;
            Gender = entity.Gender ?? string.Empty;
            BirthDate = entity.BirthDate ?? null;
            TShirtSize = entity.TShirtSize ?? string.Empty;
        }
    }
}
