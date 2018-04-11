using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.EntitesDTO
{
    [Serializable]
    public class UserDTO 
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public ICollection<GroupDTO> Groups { get; set; }
        public ICollection<RoleDTO> Roles { get; set; }
    }
}
