using AutoMapper;
using BLL.EntitesDTO;
using BLL.Interfaces;
using Model;
using Model.Entities;
using Model.Helpers;
using Model.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace BLL.Services
{
    public class AdministrationService : IAdministrationService
    {
        private WPFOutlookContext _context;
        private IGenericRepository<Log> _logs;
        private IGenericRepository<User> _users;
        private IGenericRepository<Group> _groups;
        private IGenericRepository<Role> _roles;

        public AdministrationService(IGenericRepository<User> users, IGenericRepository<Role> roles, IGenericRepository<Group> groups, IGenericRepository<Log> logs, WPFOutlookContext context)
        {
            _logs = logs;
            _context = context;
            _users = users;
            _roles = roles;
            _groups = groups;
        }

        #region GroupMethods

        public void CreateGroup(GroupDTO groupDTO, ICollection<GroupDTO> groups, ICollection<UserDTO> users, int id)
        {
            var groupItem = Mapper.Map<GroupDTO, Group>(groupDTO);
            groupItem.CreatorId = id;
            groupItem.Creator = _users.FindById(id);

            groupItem.Users = new List<User>();
            var convertUsers = Mapper.Map<IEnumerable<UserDTO>, IEnumerable<User>>(users);
            foreach (var item in convertUsers)
            {
                if (_users.FindById(item.UserId) != null)
                {
                    groupItem.Users.Add(_users.FindById(item.UserId));
                }
            }

            _groups.Create(groupItem);
            _context.SaveChanges();

            var convertGroups = new List<Group>();
            if (groups != null)
            {
                var convert = Mapper.Map<IEnumerable<GroupDTO>, IEnumerable<Group>>(groups);
                foreach (var item in convert)
                {
                    if (_groups.FindById(item.GroupId) != null)
                    {
                        convertGroups.Add(_groups.FindById(item.GroupId));
                    }
                }
            }

            foreach (var item in convertGroups)
            {
                item.ParentId = groupItem.GroupId;
            }

            _context.SaveChanges();
        }

        public void DeleteGroup(int id)
        {
            Group group = _groups.FindById(id);
            ICollection<Group> childs = _groups.Get(g => g.ParentId == group.GroupId).ToList();
            if (group.ParentId != null)
            {
                foreach (var item in childs)
                {
                    Group child = _groups.FindById(item.GroupId);
                    child.ParentId = group.ParentId;
                }
            }
            else
            {
                foreach (var item in childs)
                {
                    Group child = _groups.FindById(item.GroupId);
                    child.ParentId = null;
                    _groups.Update(child);
                }
            }
            _groups.Remove(group);
            _context.SaveChanges();
        }

        public ICollection<string> GetGroupAncestors(string groupName)
        {
            ICollection<string> ancstrorNameList = new List<string>();

            Group group;
            while (groupName != null)
            {
                ancstrorNameList.Add(groupName);
                group = _groups.Get(g => g.GroupName.Equals(groupName)).FirstOrDefault();

                if (group != null && group.ParentId != null)
                {
                    groupName = _groups.Get(g => g.GroupId == group.ParentId).FirstOrDefault().GroupName;
                }
                else { groupName = null; }
            }
            return ancstrorNameList;
        }

        public ICollection<int> GetGroupChildren(int id)
        {
            SqlParameter param = new SqlParameter("@groupId", id);
            var groups = _context.Database.SqlQuery<int>("GetGroupChilds @groupId", param).ToList();
            return groups.Distinct().ToList();
        }

        public bool CheckGroup(string groupName)
        {
            if (_groups.Get(g => g.GroupName.ToLower().Equals(groupName.ToLower())).Any())
                return false;
            else return true;
        }

        public ICollection<GroupDTO> GetGroupFirstGeneration(int id)
        {
            return Mapper.Map<IEnumerable<Group>, ICollection<GroupDTO>>(_groups.Get(g => g.ParentId == id));
        }

        private void DeleteUsersFromBranch(ICollection<User> users, int id)
        {
            SqlParameter param1 = new SqlParameter("@groupId", id);
            var dataTable = new DataTable();
            dataTable.Columns.Add("UserId", typeof(int));
            foreach (var item in users.Select(u => u.UserId))
            {
                dataTable.Rows.Add(item);
            }
            SqlParameter param2 = new SqlParameter("@List", SqlDbType.Structured);
            param2.Value = dataTable;
            param2.TypeName = "dbo.UserList";
            var res = _context.Database.ExecuteSqlCommand("DeleteUsersFromGroupChilds @groupId,@List", param1, param2);
        }

        private ICollection<User> GetGroupUsersFromBranches(int id)
        {
            List<User> users = new List<User>();
            foreach (var item in GetGroupChildren(id))
            {
                users.AddRange(_groups.FindById(item).Users);
            }
            users.AddRange(_groups.FindById(id).Users);
            return users.GroupBy(u => u.UserId).Select(g => g.First()).ToList();
        }

        public void EditGroup(GroupDTO group, ICollection<GroupDTO> selectedGroups, ICollection<UserDTO> selectedUsers)
        {
            if (group.GroupName != null)
            {
                Group groupToEdit = _groups.FindById(group.GroupId);
                if (group.GroupName != null && group.GroupName != groupToEdit.GroupName) groupToEdit.GroupName = group.GroupName;
                if (group.ParentId != groupToEdit.ParentId) groupToEdit.ParentId = group.ParentId;
                ICollection<User> usersFromBranches = GetGroupUsersFromBranches(groupToEdit.GroupId);
                if (selectedUsers.Any())
                {
                    var convertUsers = new List<User>();
                    if (selectedUsers != null)
                    {
                        var convert = Mapper.Map<IEnumerable<UserDTO>, ICollection<User>>(selectedUsers);
                        foreach (var item in convert)
                        {
                            if (_users.FindById(item.UserId) != null)
                            {
                                convertUsers.Add(_users.FindById(item.UserId));
                            }
                        }
                    }

                    groupToEdit.Users = groupToEdit.Users.Intersect(convertUsers).ToList();
                    foreach (var oldUser in usersFromBranches.ToList())
                    {
                        foreach (var user in selectedUsers.ToList())
                        {
                            if (user.UserId == oldUser.UserId)
                            {
                                usersFromBranches.Remove(oldUser);
                                selectedUsers.Remove(user);
                            }
                        }
                    }
                    if (usersFromBranches.Any())
                    {
                        DeleteUsersFromBranch(usersFromBranches, groupToEdit.GroupId);
                    }
                    if (selectedUsers.Any())
                    {
                        foreach (var item in selectedUsers.ToList())
                        {
                            groupToEdit.Users.Add(_users.FindById(item.UserId));
                        }
                    }
                }
                else
                {
                    foreach (var item in groupToEdit.Users.ToList())
                    {
                        groupToEdit.Users.Remove(_users.FindById(item.UserId));
                    }
                    if (usersFromBranches.Any())
                        DeleteUsersFromBranch(usersFromBranches, groupToEdit.GroupId);
                }
                ICollection<Group> oldGroups = _groups.Get(g => g.ParentId == groupToEdit.GroupId).ToList();
                foreach (var item in oldGroups.ToList())
                {
                    foreach (var temp in selectedGroups.ToList())
                    {
                        if (item.GroupId == temp.GroupId)
                        {
                            oldGroups.Remove(item);
                            selectedGroups.Remove(temp);
                        }
                    }
                }
                if (oldGroups.Any())
                {
                    foreach (var item in oldGroups)
                    {
                        var temp = _groups.FindById(item.GroupId);
                        temp.ParentId = null;
                    }
                }
                if (selectedGroups.Any())
                {
                    var convertGroups = new List<Group>();
                    if (selectedGroups != null)
                    {
                        var convert = Mapper.Map<IEnumerable<GroupDTO>, IEnumerable<Group>>(selectedGroups);
                        foreach (var item in convert)
                        {
                            if (_groups.FindById(item.GroupId) != null)
                            {
                                convertGroups.Add(_groups.FindById(item.GroupId));
                            }
                        }
                    }

                    foreach (var item in convertGroups)
                    {
                        item.ParentId = groupToEdit.GroupId;
                    }
                }
                _context.SaveChanges();
                foreach (var id in usersFromBranches.Select(u => u.UserId))
                {
                    _context.Entry(_users.FindById(id)).State = System.Data.Entity.EntityState.Detached;
                }
            }
        }

        public ICollection<GroupDTO> GetGroupsWithNoAncestors()
        {
            List<Group> collection;
            using (_logs.BeginTransaction())
            {
                collection = _groups.Get(g => g.ParentId == null).ToList();
            }
            var mappingCollection = Mapper.Map<IEnumerable<Group>, ICollection<GroupDTO>>(collection).ToList();
            foreach (var item in mappingCollection)
            {
                item.CreatorName = GetUserById(item.CreatorId).Name;
                item.ParentName = GetGroupName(item.ParentId);
                item.Groups = GetGroupFirstGeneration(item.GroupId);
                item.Users = new ObservableCollection<UserDTO>(GetGroupUsers(item.GroupId));
            }
            return mappingCollection;
        }

        public ICollection<UserDTO> GetGroupUsers(int id)
        {
            ICollection<UserDTO> users = Mapper.Map<IEnumerable<User>, ICollection<UserDTO>>(GetGroupUsersFromBranches(id));
            return users;
        }

        public GroupDTO GetGroupById(int? id)
        {
            if (id != null)
            {
                return null;
            }
            else return Mapper.Map<Group, GroupDTO>(_groups.FindById((int)id));
        }

        public string GetGroupName(int? id)
        {
            if (id == null)
            {
                return string.Empty;
            }
            else return _groups.FindById((int)id).GroupName;

        }

        #endregion

        #region UserMethods

        public void CreateUser(UserDTO user, ICollection<GroupDTO> groups, ICollection<RoleDTO> roles)
        {
            var salt = EncryptionHelpers.GenerateSalt();

            var userItem = Mapper.Map<UserDTO, User>(user);
            userItem.Salt = salt;
            userItem.Password = EncryptionHelpers.HashPassword(user.Password, salt);

            userItem.Roles = new List<Role>();
            var convertRoles = Mapper.Map<IEnumerable<RoleDTO>, IEnumerable<Role>>(roles);
            foreach (var item in convertRoles)
            {
                if (_roles.FindById(item.RoleId) != null)
                {
                    userItem.Roles.Add(_roles.FindById(item.RoleId));
                }
            }

            userItem.Groups = new List<Group>();
            var convertGroups = Mapper.Map<IEnumerable<GroupDTO>, IEnumerable<Group>>(groups);
            foreach (var item in convertGroups)
            {
                if (_groups.FindById(item.GroupId) != null)
                {
                    userItem.Groups.Add(_groups.FindById(item.GroupId));
                }
            }

            _users.Create(userItem);
            _context.SaveChanges();
        }

        public void EditUser(UserDTO user, ICollection<GroupDTO> groups, ICollection<RoleDTO> roles)
        {
            if (user.UserName != null && user.Password != null)
            {
                User userToEdit = _users.FindById(user.UserId);
                if (user.Name != null) userToEdit.Name = user.Name;
                if ((user.UserName != null || userToEdit.UserName == user.UserName) && CheckUser(user.UserName)) userToEdit.UserName = user.UserName;
                if (user.Password != null && user.Password != userToEdit.Password)
                {
                    var salt = EncryptionHelpers.GenerateSalt();
                    userToEdit.Salt = salt;
                    userToEdit.Password = EncryptionHelpers.HashPassword(user.Password, salt);
                }
                if (user.IsActive != userToEdit.IsActive) userToEdit.IsActive = user.IsActive;

                var convertRoles = new List<Role>();
                if (roles != null)
                {
                    var convert = Mapper.Map<IEnumerable<RoleDTO>, IEnumerable<Role>>(roles);
                    foreach (var item in convert)
                    {
                        if (_roles.FindById(item.RoleId) != null)
                        {
                            convertRoles.Add(_roles.FindById(item.RoleId));
                        }
                    }
                }

                if (roles.Any()) userToEdit.Roles = convertRoles;
                if (!roles.Any()) userToEdit.Roles = null;

                var convertGroups = new List<Group>();
                if (groups != null)
                {
                    var convert = Mapper.Map<IEnumerable<GroupDTO>, IEnumerable<Group>>(groups);
                    foreach (var item in convert)
                    {
                        if (_groups.FindById(item.GroupId) != null)
                        {
                            convertGroups.Add(_groups.FindById(item.GroupId));
                        }
                    }
                }

                if (groups.Any()) userToEdit.Groups = convertGroups;
                if (!groups.Any()) userToEdit.Groups = null;
                _users.Update(userToEdit);
                _context.SaveChanges();
            }
        }

        public bool CheckUser(string username)
        {
            if (username == null || _users.Get(u => u.UserName.ToLower().Equals(username.ToLower())).Any())
                return false;
            else return true;
        }

        public void DeactivateUser(int id)
        {
            User user = _users.FindById(id);
            if (user.IsActive == true)
                user.IsActive = false;
            else user.IsActive = true;
            _context.SaveChanges();
        }

        public UserDTO GetUserById(int id)
        {
            return Mapper.Map<User, UserDTO>(_users.Get(u => u.UserId == id).FirstOrDefault());
        }

        public ICollection<RoleDTO> GetUserRoles(int id)
        {
            return Mapper.Map<IEnumerable<Role>, ICollection<RoleDTO>>(_users.FindById(id).Roles);
        }

        public ICollection<GroupDTO> GetUserGroups(int id)
        {
            return Mapper.Map<IEnumerable<Group>, ICollection<GroupDTO>>(_users.FindById(id).Groups);
        }

        #endregion

        public ICollection<UserDTO> GetUsers()
        {
            List<User> collection;
            using (_logs.BeginTransaction())
            {
                collection = _users.Get().ToList();
            }
            var mappingCollection = Mapper.Map<IEnumerable<User>, ICollection<UserDTO>>(collection).ToList();
            foreach (var item in mappingCollection)
            {
                item.Groups = GetUserGroups(item.UserId);
                item.Roles = GetUserRoles(item.UserId);
            }
            return mappingCollection;
        }

        public ICollection<GroupDTO> GetGroups()
        {
            List<Group> collection;
            using (_logs.BeginTransaction())
            {
                collection = _groups.Get().ToList();
            }
            var mappingCollection = Mapper.Map<IEnumerable<Group>, ICollection<GroupDTO>>(collection).ToList();
            foreach (var item in mappingCollection)
            {
                item.CreatorName = GetUserById(item.CreatorId).Name;
                item.ParentName = GetGroupName(item.ParentId);
                item.Groups = GetGroupFirstGeneration(item.GroupId);
                item.Users = GetGroupUsers(item.GroupId);
            }
            return mappingCollection;
        }

        public ICollection<RoleDTO> GetRoles()
        {
            return Mapper.Map<IEnumerable<Role>, ICollection<RoleDTO>>(_roles.Get());
        }

        public ICollection<LogDTO> GetLogs()
        {
            List<Log> collection;
            using (_logs.BeginTransaction())
            {
                collection = _logs.Get().ToList();
            }
            var mappingCollection = Mapper.Map<IEnumerable<Log>, ICollection<LogDTO>>(collection).ToList();
            foreach (var item in mappingCollection)
            {
                item.ActionAuthorName = GetUserById(item.ActionAuthorId).Name;
                item.CreatorName = GetUserById(item.CreatorId).Name;
            }
            return mappingCollection;
        }

        public int GetNumberOfAdmins()
        {
            return _users.Get(u => u.IsActive && (u.Roles.Where(r => r.Name == "admin")).Any()).Count();
        }
    }
}