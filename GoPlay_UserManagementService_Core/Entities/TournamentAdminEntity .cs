using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay_UserManagementService_Core.Entities
{
    public class TournamentAdmin : UserDto
    {
        public string Cnpj { get; set; }

        public TournamentAdmin()
        {
            
        }

        public TournamentAdmin(int idUser, string name, string email, string login, string password, string? instagramPage, int userTypeId, string cnpj)
        {
            IdUser = idUser;
            Name = name;
            Email = email;
            Login = login;
            Password = password;
            InstagramPage = instagramPage;
            UserTypeId = userTypeId;
            Cnpj = cnpj;
        }
    }
}
