using AutoMapper;
using BLL.EntitesDTO;
using BLL.Interfaces;
using BLL.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ViewModel.Models;
using ViewModel.ViewModels.Administration.Groups;

namespace ViewModel.ViewModels.CommonViewModels.Groups
{
    [PrincipalPermission(SecurityAction.Demand, Role = "admin")]
    public class ShowAllGroupsViewModel : ViewModelBase
    {
        private readonly IAdministrationService _administrationService;
        private ObservableCollection<GroupModel> _groups;
        private RelayCommand<GroupModel> _editUserCommand { get; }
        private RelayCommand _addUserCommand { get; }
        private RelayCommand<GroupModel> _deleteGroupCommand { get; }

        public ShowAllGroupsViewModel(IAdministrationService administationService)
        {
            _administrationService = administationService;
            _groups = new ObservableCollection<GroupModel>();
            LoadData();
            _editUserCommand = new RelayCommand<GroupModel>(EditGroup);
            _addUserCommand = new RelayCommand(AddGroup);
            _deleteGroupCommand = new RelayCommand<GroupModel>(DeleteGroup);
        }

        public RelayCommand<GroupModel> EditUserCommand { get { return _editUserCommand; } }

        public RelayCommand AddUserCommand { get { return _addUserCommand; } }

        public RelayCommand<GroupModel> DeleteGroupCommand { get { return _deleteGroupCommand; } }

        public ObservableCollection<GroupModel> Groups
        {
            get => _groups;
            set
            {
                try
                {
                    _groups = value;
                    base.RaisePropertyChanged();
                }
                catch (NullReferenceException ex)
                {
                    MessageBox.Show(ex.StackTrace);
                }
            }
        }

        private void DeleteGroup(GroupModel group)
        {
            if (group != null)
            {
                if (group.ParentId == null)
                {
                    foreach (var item in group.Groups)
                    {
                        Groups.Add(item);
                    }
                    Groups.Remove(group);
                }
                else
                {
                    var parent = FindParentGroup((int)group.ParentId);
                    foreach (var item in group.Groups)
                    {
                        parent.Groups.Add(item);
                    }
                    parent.Groups.Remove(group);
                }
                _administrationService.DeleteGroup(group.GroupId);
            }
        }

        private GroupModel FindParentGroup(int id)
        {
            GroupModel group = new GroupModel();
            foreach(var item in Groups)
            {
                if (item.GroupId == id)
                {
                    group = item;
                    break;
                }
                else
                {
                    var finding = SearchBranch(item.Groups, id);
                    if (!string.IsNullOrEmpty(finding.GroupName))
                    {
                        group = finding;
                        break;
                    }
                }
            }
            return group;
        }

        private GroupModel SearchBranch(ICollection<GroupModel> groups, int id)
        {
            GroupModel group = new GroupModel();
            foreach (var item in groups)
            {
                if (item.GroupId == id)
                {
                    group = item;
                    break;
                }
                else
                {
                    var finding = SearchBranch(item.Groups, id);
                    if (finding != null)
                    {
                        group = finding;
                        break;
                    }   
                }
            }
            return group;
        }

        private void AddGroup()
        {
            Messenger.Default.Send(new NotificationMessage("AddGroupWindow"));
            LoadData();
        }

        private void EditGroup(GroupModel group)
        {
            if (group != null)
            {
                Messenger.Default.Send<GroupModel>(group);
                LoadData();


                //foreach(var item in group.Groups)
                //{
                //    foreach(var grp in Groups.ToList())
                //    {
                //        if(item.GroupName == grp.GroupName)
                //        {
                //            Groups.Remove(grp);
                //        }
                //    }
                //}
            }
        }

        private void LoadData()
        {
            Groups = new ObservableCollection<GroupModel>(Mapper.Map<IEnumerable<GroupDTO>, ICollection<GroupModel>>(_administrationService.GetGroupsWithNoAncestors()));
            var allGroups = Mapper.Map<IEnumerable<GroupDTO>, ICollection<GroupModel>>(_administrationService.GetGroups());
            foreach(var item in Groups)
            {
                foreach(var group in allGroups.ToList())
                {
                    if (item.GroupId == group.GroupId)
                        allGroups.Remove(group);
                }
            }
            while (allGroups.Any())
            {
                foreach (var item in Groups)
                {
                    allGroups = FillChildrens(allGroups, item.Groups).ToList();
                }
            }
        }

        private IEnumerable<GroupModel> FillChildrens(ICollection<GroupModel> allGroups, ICollection<GroupModel> groupModel)
        {
            if (groupModel != null)
            {   
                foreach (var item in groupModel)
                {
                    allGroups = allGroups.Where(g => g.GroupId != item.GroupId).ToList();
                    var groups = _administrationService.GetGroupFirstGeneration(item.GroupId);
                    if (groups.Any())
                    {
                        item.Groups = new ObservableCollection<GroupModel>(Mapper.Map<IEnumerable<GroupDTO>, ICollection<GroupModel>>(groups));
                        foreach (var temp in item.Groups)
                        {
                            foreach (var group in allGroups.ToList())
                            {
                                if (temp.GroupId == group.GroupId)
                                    allGroups.Remove(group);
                            }
                        }
                        allGroups = FillChildrens(allGroups, item.Groups).ToList();
                    }
                }
            }
            return allGroups;

        }
    }
}