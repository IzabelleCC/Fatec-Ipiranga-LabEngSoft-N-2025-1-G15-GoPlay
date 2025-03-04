using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay_UserManagementService_Core.Entities
{
    public class UserTypeEntity
    {
        public int UserTypeId { get; set; }
        public string Description { get; set; }
        public UserTypeEntity()
        {
        }
        public UserTypeEntity(int UserTypeId, string description)
        {
            UserTypeId = UserTypeId;
            Description = description;
        }
    }
}
