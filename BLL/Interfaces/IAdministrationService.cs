using BLL.EntitesDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAdministrationService
    {
        void CreateUser(UserDTO user, ICollection<GroupDTO> groups, ICollection<RoleDTO> roles);
        void EditUser(UserDTO user, ICollection<GroupDTO> groups, ICollection<RoleDTO> roles);
        ICollection<RoleDTO> GetUserRoles(int id);
        ICollection<GroupDTO> GetUserGroups(int id);
        int GetNumberOfAdmins();
        UserDTO GetUserById(int id);
        GroupDTO GetGroupById(int? id);
        string GetGroupName(int? id);
        ICollection<string> GetGroupAncestors(string groupName);
        ICollection<int> GetGroupChildren(int id);
        ICollection<LogDTO> GetLogs();
        bool CheckUser(string username);
        bool CheckGroup(string groupName);
        void DeactivateUser(int id);
        void CreateGroup(GroupDTO group, ICollection<GroupDTO> Groups, ICollection<UserDTO> users, int id);
        void EditGroup(GroupDTO group, ICollection<GroupDTO> Groups, ICollection<UserDTO> users);
        void DeleteGroup(int id);
        ICollection<GroupDTO> GetGroupFirstGeneration(int id);
        ICollection<UserDTO> GetGroupUsers(int id);
        ICollection<GroupDTO> GetGroupsWithNoAncestors();
        ICollection<RoleDTO> GetRoles();
        ICollection<GroupDTO> GetGroups();
        ICollection<UserDTO> GetUsers();
    }
}
