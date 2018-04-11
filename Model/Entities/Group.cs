using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities
{

    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        //[Key]
        //public int RelatedId { get; set; }
        public string GroupName { get; set; }

        public int? ParentId { get; set; }
        public Group Parent { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
