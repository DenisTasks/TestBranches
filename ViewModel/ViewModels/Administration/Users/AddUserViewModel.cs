using AutoMapper;
using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ViewModel.Models;

namespace ViewModel.ViewModels.Administration.Users
{
    public class AddUserViewModel : ViewModelBase
    {
        private readonly IAdministrationService _administrationService;

        private ObservableCollection<RoleDTO> _roleList;

        private ObservableCollection<GroupDTO> _groupList;

        public UserModel User { get; set; }

        public AddUserViewModel(IAdministrationService administrationService)
        {
            _administrationService = administrationService;
            _roleList = new ObservableCollection<RoleDTO>(_administrationService.GetRoles());

            _groupList = new ObservableCollection<GroupDTO>(_administrationService.GetGroups());

            _addRoleCommand = new RelayCommand<RoleDTO>(AddRole);
            _removeRoleCommand = new RelayCommand<RoleDTO>(RemoveRole);
            _addGroupCommand = new RelayCommand<GroupDTO>(AddGroup);
            _removeGroupCommand = new RelayCommand<GroupDTO>(RemoveGroup);
            _createUserCommand = new RelayCommand<Window>(CreateUser);

            User = new UserModel();
        }

        public ObservableCollection<RoleDTO> RoleList
        {
            get => _roleList;
            set
            {
                if (value!= _roleList)
                {
                    _roleList = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<GroupDTO> GroupList
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
        
        private RelayCommand<RoleDTO> _addRoleCommand;
        private RelayCommand<RoleDTO> _removeRoleCommand;

        public RelayCommand<RoleDTO> AddRoleCommand { get { return _addRoleCommand; } }
        public RelayCommand<RoleDTO> RemoveRoleCommand { get { return _removeRoleCommand; } }

        public void AddRole(RoleDTO role)
        {
            User.Roles.Add(role);
            RoleList.Remove(role);
            base.RaisePropertyChanged();
        }

        public void RemoveRole(RoleDTO role)
        {
            RoleList.Add(role);
            User.Roles.Remove(role);
            base.RaisePropertyChanged();
        }


        private RelayCommand<GroupDTO> _addGroupCommand;
        private RelayCommand<GroupDTO> _removeGroupCommand;

        public RelayCommand<GroupDTO> AddGroupCommand { get { return _addGroupCommand; } }
        public RelayCommand<GroupDTO> RemoveGroupCommand { get { return _removeGroupCommand; } }

        public void AddGroup(GroupDTO group)
        {
            User.Groups.Add(group);
            GroupList.Remove(group);
            base.RaisePropertyChanged();
        }

        public void RemoveGroup(GroupDTO group)
        {
            GroupList.Add(group);
            User.Groups.Remove(group);
            base.RaisePropertyChanged();
        }

        private RelayCommand<Window> _createUserCommand;

        public RelayCommand<Window> CreateUserCommand { get { return _createUserCommand; } }

        public void CreateUser(Window window)
        {
            if (User.UserName != null && User.Password != null)
            {
                if (_administrationService.CheckUser(User.UserName))
                {
                    _administrationService.CreateUser(Mapper.Map<UserModel, UserDTO>(User), User.Groups, User.Roles);
                    window.Close();
                }
                else
                {
                    MessageBox.Show("User with that username already exists!");
                }
            } else
            {
                MessageBox.Show("Fill empty fields!");
            }
        }

    }
}
