using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using GoPlay_Core.Enum;
using Microsoft.AspNetCore.Identity;


namespace GoPlay_Core.Entities
{
    public class UserEntity : IdentityUser
    {
        public string Name { get; set; }
        public UserTypeEnum UserType { get; set; }
        public string? InstagramPage { get; set; }
        public string CpfCnpj { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? TShirtSize { get; set; }

        public UserEntity()
        {

        }
    }
}
