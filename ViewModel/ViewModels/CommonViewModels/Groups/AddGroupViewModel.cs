using AutoMapper;
using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ViewModel.Models;
using ViewModel.ViewModels.Authenication;

namespace ViewModel.ViewModels.CommonViewModels.Groups
{
    public class AddGroupViewModel : ViewModelBase
    {
        private readonly IAdministrationService _administrationService;
        private readonly CustomPrincipal _customPrincipal;

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
            }
        }

        private void FilterGroupList(GroupModel group)
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
                    
                    foreach(var item in GroupList.ToList() )
                    {
                        if (item.GroupName == groupName)
                        {
                            GroupList.Remove(item);
                            _hiddenGroupAncestors.Add(item);
                        }
                    }
                }
                Group.Groups = new ObservableCollection<GroupModel>();
            }
            else
            {
                Group.ParentId = null;
                Group.Groups = new ObservableCollection<GroupModel>();
                foreach(var item in _hiddenGroupAncestors.ToList())
                {
                    _hiddenGroupAncestors.Remove(item);
                }
                GroupList = new ObservableCollection<GroupModel>(_groupsWithNoAncestors);
            }
        }

        public GroupModel Group { get; set; }

        public AddGroupViewModel(IAdministrationService administrationService)
        {
            _administrationService = administrationService;

            _hiddenGroupAncestors = new List<GroupModel>();

            _userList = new ObservableCollection<UserDTO>(_administrationService.GetUsers());

            _groupsForComboBox = Mapper.Map<IEnumerable<GroupDTO>, ICollection<GroupModel>>(_administrationService.GetGroups());
            _groupsWithNoAncestors = Mapper.Map<IEnumerable<GroupDTO>, ICollection<GroupModel>>(_administrationService.GetGroupsWithNoAncestors());
            _groupList = new ObservableCollection<GroupModel>(_groupsWithNoAncestors);
            _groupsForComboBox.Add(new GroupModel { GroupName = "Not" }); 

            _addUserCommand = new RelayCommand<UserDTO>(AddUser);
            _removeUserCommand = new RelayCommand<UserDTO>(RemoveUser);
            _addGroupCommand = new RelayCommand<GroupModel>(AddGroup);
            _removeGroupCommand = new RelayCommand<GroupModel>(RemoveGroup);
            _createGroupCommand = new RelayCommand<Window>(CreateGroup);

            _customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;

            Group = new GroupModel();

            AddUser(UserList.FirstOrDefault(u => u.UserId == _customPrincipal.Identity.UserId));
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
            Group.Groups.Add(group);
            GroupList.Remove(group);
            base.RaisePropertyChanged();
        }

        public void RemoveGroup(GroupModel group)
        {
            Group.Groups.Remove(group);
            GroupList.Add(group);
            base.RaisePropertyChanged();
        }

        private RelayCommand<Window> _createGroupCommand;

        public RelayCommand<Window> CreateGroupCommand { get { return _createGroupCommand; } }

        public void CreateGroup(Window window)
        {
            if (Group.GroupName!=null)
            {
                if (_administrationService.CheckGroup(Group.GroupName))
                {
                    _administrationService.CreateGroup(Mapper.Map<GroupModel, GroupDTO>(Group), Mapper.Map<IEnumerable<GroupModel>, ICollection<GroupDTO>>(Group.Groups), Group.Users, _customPrincipal.Identity.UserId);
                    window.Close();
                }
                else
                {
                    MessageBox.Show("Group with this name already exists");
                }
            }
            else
            {
                MessageBox.Show("Fill empty fields!");
            }
        }
    }
}

