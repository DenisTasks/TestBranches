using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.EntitesDTO
{
    public class GroupDTO
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int? ParentId { get; set; }
        public int CreatorId { get; set; }

        public string CreatorName { get; set; }
        public string ParentName { get; set; }
        public ICollection<GroupDTO> Groups { get; set; }
        public ICollection<UserDTO> Users { get; set; }
    }
}
