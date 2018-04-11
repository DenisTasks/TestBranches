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

namespace ViewModel.ViewModels.Administration.Users
{
    public class EditUserViewModel : ViewModelBase
    {
        private readonly IAdministrationService _administrationService;

        private UserModel _user;
        private string _oldUserName;
        private string _oldPassword;
        private string _password;

        private ObservableCollection<RoleDTO> _roleList;

        private ObservableCollection<GroupDTO> _groupList;

        public UserModel User
        {
            get => _user;
            set
            {
                if (value != null)
                {
                    _user = value; base.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<RoleDTO> RoleList
        {
            get => _roleList;
            set
            {
                if (value != null)
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
                if (value != null)
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
            if (role.Name.Equals("admin") && _administrationService.GetNumberOfAdmins() == 1 && User.IsActive)
            {
                MessageBox.Show("You need to have one or more admin usres in the system");
            }
            else
            {
                RoleList.Add(role);
                User.Roles.Remove(role);
                base.RaisePropertyChanged();
            }
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

        private RelayCommand<Window> _editUserCommand;

        public RelayCommand<Window> EditUserCommand { get { return _editUserCommand; } }

        public void EditUser(Window window)
        {
            if (_password.Equals(User.Password))
            {
                User.Password = _oldPassword;
            }
            if (_oldUserName == User.UserName && User.Password !=null)
            {
                _administrationService.EditUser(Mapper.Map<UserModel, UserDTO>(User), User.Groups, User.Roles);
                window.Close();
            }
            else
            {
                if (User.UserName != null && User.Password!=null)
                {
                    if (_administrationService.CheckUser(User.UserName))
                    {
                        _administrationService.EditUser(Mapper.Map<UserModel, UserDTO>(User), User.Groups, User.Roles);
                        window.Close();
                    }
                    else
                    {
                        MessageBox.Show("User with this name already exists");
                    }
                }
                else
                {
                    MessageBox.Show("Fill empty fields!");
                }
            }
        }


        public EditUserViewModel(IAdministrationService administrationService)
        {
            _password = "Enter new password if needed";
            Messenger.Default.Register<UserModel>(this, user =>
            {
                if (user != null)
                {
                    User = user;
                    _oldUserName = user.UserName;
                    _oldPassword = user.Password;
                    User.Password = _password;

                    RoleList = new ObservableCollection<RoleDTO>(_administrationService.GetRoles());
                    foreach (var item in User.Roles)
                    {
                        foreach(var temp in RoleList.ToList())
                        {
                            if(item.Name == temp.Name)
                            {
                                RoleList.Remove(temp);
                            }
                        }
                    }
                    
                    GroupList = new ObservableCollection<GroupDTO>(_administrationService.GetGroups());
                    foreach (var item in User.Groups)
                    {
                        foreach (var temp in GroupList.ToList())
                        {
                            if (item.GroupName == temp.GroupName)
                            {
                                GroupList.Remove(temp);
                            }
                        }
                    }
                    Messenger.Default.Unregister<UserModel>(this);
                }
            });
            _administrationService = administrationService;
            
            _addRoleCommand = new RelayCommand<RoleDTO>(AddRole);
            _removeRoleCommand = new RelayCommand<RoleDTO>(RemoveRole);
            _addGroupCommand = new RelayCommand<GroupDTO>(AddGroup);
            _removeGroupCommand = new RelayCommand<GroupDTO>(RemoveGroup);
            _editUserCommand = new RelayCommand<Window>(EditUser);
        }
    }
}

