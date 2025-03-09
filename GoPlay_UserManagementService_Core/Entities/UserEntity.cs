using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using GoPlay_UserManagementService_Core.Enum;


namespace GoPlay_UserManagementService_Core.Entities
{
    public class UserEntity 
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public UserTypeEnum UserType { get; set; }
        public string? InstagramPage { get; set; }
        public string? Cpf { get; set; }
        public string? Cnpj { get; set; }
        public string? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? TShirtSize { get; set; }


        public UserEntity()
        {

        }


    }
}
