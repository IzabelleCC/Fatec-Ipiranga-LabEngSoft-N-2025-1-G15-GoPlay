using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace GoPlay_UserManagementService_Core.Entities
{
    public class PlayerEntity : UserEntity
    {
        public string Cpf { get; set; }
        public string Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? TShirtSize { get; set; }

        public PlayerEntity()
        {
            
        }

        public PlayerEntity(int idUser, string name, string email, string login, string password, string? instagramPage, int userTypeId, string cpf, string gender, DateOnly birthDate, string? tShirtSize)
        {
            IdUser = idUser;
            Name = name;
            Email = email;
            Login = login;
            Password = password;
            InstagramPage = instagramPage;
            UserTypeId = userTypeId;
            Cpf = cpf;
            Gender = gender;
            BirthDate = birthDate;
            TShirtSize = tShirtSize;
        }



    }
}
