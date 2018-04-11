using AutoMapper;
using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModel.Models;

namespace ViewModel.ViewModels.Administration.Groups
{
    public class EditGroupViewModel : ViewModelBase
    {
        private readonly IAdministrationService _administrationService;

        private bool _editor;
        private string _oldName;
        private ICollection<GroupModel> _hiddenGroupAncestors;
        private ICollection<GroupModel> _groupsWithNoAncestors;
        private ObservableCollection<GroupModel> _groupList;
        private ObservableCollection<UserDTO> _userList;
        private ICollection<GroupModel> _groupsForComboBox;

        public ICollection<GroupModel> GroupsForComboBox
        {
            get => _groupsForComboBox;
            set
            {
                _groupsForComboBox = value;
            }
        }

        private GroupModel _groupNameForFilter;

        public GroupModel GroupNameForFilter
        {
            get => _groupNameForFilter;
            set
            {
                _groupNameForFilter = value;
                FilterGroupList(_groupNameForFilter);
                base.RaisePropertyChanged();
            }
        }

        private void FilterGroupList(GroupModel group)
        {
            if (_editor)
            {
                if (group.GroupName != "Not")
                {
                    Group.ParentId = group.GroupId;
                    ICollection<string> groupNameList = _administrationService.GetGroupAncestors(group.GroupName);
                    if (_hiddenGroupAncestors.Any())
                    {
                        foreach (var item in _hiddenGroupAncestors.GroupBy(g => g.GroupId).First().ToList())
                        {
                            GroupList.Add(item);
                            _hiddenGroupAncestors.Remove(item);
                        }
                    }
                    foreach (var groupName in groupNameList)
                    {
                        foreach (var item in GroupList.ToList())
                        {
                            if (item.GroupName == groupName)
                            {
                                GroupList.Remove(item);
                                _hiddenGroupAncestors.Add(item);
                            }
                        }
                    }
                    foreach (var item in _hiddenGroupAncestors.Select(g => g.GroupName).Except(groupNameList).ToList())
                    {
                        var temp = _hiddenGroupAncestors.FirstOrDefault(x => x.GroupName == item);
                        _hiddenGroupAncestors.Remove(temp);
                        GroupList.Add(temp);
                    }
                    Group.Groups = new ObservableCollection<GroupModel>();
                }
                else
                {
                    _groupsWithNoAncestors= _groupsWithNoAncestors.GroupBy(g => g.GroupId).Select(x => x.First()).ToList();
                    Group.ParentId = null;
                    foreach (var item in _hiddenGroupAncestors.ToList())
                    {
                        _hiddenGroupAncestors.Remove(item);
                        foreach(var temp in _groupsWithNoAncestors.ToList())
                        {
                            if(temp.GroupId == item.GroupId)
                            {
                                _groupsWithNoAncestors.Remove(temp);
                                GroupList.Add(item);
                            }
                        }
                    }
                }
            }
            else
            {
                _editor = true;
                if (Group.ParentId != null)
                {
                    ICollection<string> groupNameList = _administrationService.GetGroupAncestors(Group.GroupName);
                    foreach (var ancestor in groupNameList)
                    {
                        foreach (var item in GroupList.ToList())
                        {
                            if (item.GroupName == ancestor)
                            {
                                GroupList.Remove(item);
                                _hiddenGroupAncestors.Add(item);
                            }
                        }
                    }
                }
                GroupList.Remove(GroupList.FirstOrDefault(g=>g.GroupId == Group.GroupId));
            }
        }

        private GroupModel _group;

        public GroupModel Group
        {
            get => _group;
            set
            {
                if (value != null)
                {
                    _group = value; base.RaisePropertyChanged();
                }
            }
        }

        public EditGroupViewModel(IAdministrationService administrationService)
        {
            Messenger.Default.Register<GroupModel>(this, group =>
            {
                if (group != null)
                {
                    Group = group;
                    _oldName = group.GroupName;
                
                    UserList = new ObservableCollection<UserDTO>(_administrationService.GetUsers());
                    foreach (var item in Group.Users)
                    {
                        foreach (var temp in UserList.ToList())
                        {
                            if (item.Name == temp.Name)
                            {
                                UserList.Remove(temp);
                            }
                        }
                    }
                    
                    GroupList = new ObservableCollection<GroupModel>(_groupsWithNoAncestors);

                    _groupsForComboBox.Remove(_groupsForComboBox.FirstOrDefault(g => g.GroupId == Group.GroupId));
                    GroupNameForFilter = _groupsForComboBox.FirstOrDefault(g => g.GroupId == group.ParentId);
                    Messenger.Default.Unregister<GroupModel>(this);
                }
            });

            _administrationService = administrationService;
            _editor = false;
            _hiddenGroupAncestors = new List<GroupModel>();
            _groupsWithNoAncestors = Mapper.Map<IEnumerable<GroupDTO>, ICollection< GroupModel>>(_administrationService.GetGroupsWithNoAncestors());

            _groupsForComboBox = Mapper.Map<IEnumerable<GroupDTO>, ICollection<GroupModel>>(_administrationService.GetGroups());
            _groupsForComboBox.Add(new GroupModel { GroupName = "Not" });

            _addUserCommand = new RelayCommand<UserDTO>(AddUser);
            _removeUserCommand = new RelayCommand<UserDTO>(RemoveUser);
            _addGroupCommand = new RelayCommand<GroupModel>(AddGroup);
            _removeGroupCommand = new RelayCommand<GroupModel>(RemoveGroup);
            _createGroupCommand = new RelayCommand<Window>(CreateGroup);
        }

        public ObservableCollection<UserDTO> UserList
        {
            get => _userList;
            set
            {
                if (value != _userList)
                {
                    _userList = value;
                    base.RaisePropertyChanged();
                }
            }
        }
        

        public ObservableCollection<GroupModel> GroupList
        {
            get => _groupList;
            set
            {
                if (value != _groupList)
                {
                    _groupList = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        private RelayCommand<UserDTO> _addUserCommand;
        private RelayCommand<UserDTO> _removeUserCommand;

        public RelayCommand<UserDTO> AddUserCommand { get { return _addUserCommand; } }
        public RelayCommand<UserDTO> RemoveUserCommand { get { return _removeUserCommand; } }

        public void AddUser(UserDTO user)
        {
            Group.Users.Add(user);
            UserList.Remove(user);
            base.RaisePropertyChanged();
        }

        public void RemoveUser(UserDTO user)
        {
            UserList.Add(user);
            Group.Users.Remove(user);
            base.RaisePropertyChanged();
        }


        private RelayCommand<GroupModel> _addGroupCommand;
        private RelayCommand<GroupModel> _removeGroupCommand;

        public RelayCommand<GroupModel> AddGroupCommand { get { return _addGroupCommand; } }
        public RelayCommand<GroupModel> RemoveGroupCommand { get { return _removeGroupCommand; } }

        public void AddGroup(GroupModel group)
        {
            if (group != null)
            {
                Group.Groups.Add(group);
                GroupList.Remove(group);
                base.RaisePropertyChanged();
            }
        }

        public void RemoveGroup(GroupModel group)
        {
            if (group != null)
            {
                Group.Groups.Remove(group);
                GroupList.Add(group);
                base.RaisePropertyChanged();
            }
        }

        private RelayCommand<Window> _createGroupCommand;

        public RelayCommand<Window> CreateGroupCommand { get { return _createGroupCommand; } }

        public void CreateGroup(Window window)
        {
            if (Group.GroupName == _oldName)
            {
                _administrationService.EditGroup(Mapper.Map<GroupModel, GroupDTO>(Group), Mapper.Map<IEnumerable<GroupModel>, ICollection<GroupDTO>>(Group.Groups), Group.Users);
                window.Close();
            }
            else
            { 
                if (Group.GroupName != null)
                {
                   if( _administrationService.CheckGroup(Group.GroupName))
                    {
                        _administrationService.EditGroup(Mapper.Map<GroupModel,GroupDTO>(Group), Mapper.Map<IEnumerable<GroupModel>, ICollection<GroupDTO>>(Group.Groups), Group.Users);
                        window.Close();
                    }
                    else { MessageBox.Show("This name already exists"); }
                }
                else
                {
                    MessageBox.Show("Fill empty fields!");
                }
            }
        }
    }
}
