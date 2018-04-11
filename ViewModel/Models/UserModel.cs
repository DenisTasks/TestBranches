using BLL.EntitesDTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Models
{
    public class UserModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public int UserId { get; set; }
        private bool _isActive;
        private string _name;
        private string _userName;
        private string _password;

        private ObservableCollection<GroupDTO> _groups;
        private ObservableCollection<RoleDTO> _roles;
        private ObservableCollection<AppointmentDTO> _appointments;

        public UserModel()
        {
            _groups = new ObservableCollection<GroupDTO>();
            _roles = new ObservableCollection<RoleDTO>();
            _appointments = new ObservableCollection<AppointmentDTO>();
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                NotifyPropertyChanged("IsActive");
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                if (value != string.Empty)
                {
                    _userName = value;
                    NotifyPropertyChanged("UserName");
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if(value != string.Empty)
                {
                    _password = value;
                    NotifyPropertyChanged("Password");
                }
            }
        }

        public ObservableCollection<GroupDTO> Groups
        {
            get => _groups;
            set
            {
                _groups = value;
                NotifyPropertyChanged("Groups");
            }
        }

        public ObservableCollection<RoleDTO> Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                NotifyPropertyChanged("Roles");
            }
        }

        public ObservableCollection<AppointmentDTO> Appointments
        {
            get => _appointments;
            set
            {
                _appointments = value;
                NotifyPropertyChanged("Appointments");
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public string Error { get => null; }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "UserName")
                {
                    if (string.IsNullOrEmpty(UserName))
                        return "UserName is Required";
                }
                if (columnName == "Password")
                {
                    if (string.IsNullOrEmpty(Password))
                        return "Password is Required";
                }

                return null;
            }
        }
    }
}
