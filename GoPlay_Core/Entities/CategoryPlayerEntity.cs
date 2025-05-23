using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoPlay_Core.Entities
{
    public class CategoryPlayerEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string FirstUserId { get; set; }
        public string? SecondUserId { get; set; }
        public CategoryEntity Category { get; set; }
        public UserEntity FirstUser { get; set; }
        public UserEntity? SecondUser { get; set; }
    }

}
