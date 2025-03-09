using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace GoPlay_UserManagementService_Core.Entities
{
    public class UserEntity 
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string? InstagramPage { get; set; }
        public int UserTypeId { get; set; }
        public UserTypeEntity UserType { get; set; }

        public virtual PlayerEntity? Player { get; set; }
        public virtual TournamentAdminEntity? TournamentAdmin { get; set; }

        public UserEntity()
        {
            
        }


    }
}
