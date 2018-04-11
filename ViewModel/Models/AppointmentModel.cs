using BLL.EntitesDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ViewModel.Models
{
    [Serializable]
    public class AppointmentModel : IDataErrorInfo, INotifyPropertyChanged
    {
        private string _subject;
        private int _locationId;
        private DateTime _beginningDate;
        private DateTime _endingDate;
        private ICollection<UserDTO> _users;

        public int AppointmentId { get; set; }
        public string Subject
        {
            get { return _subject; }
            set
            {
                _subject = value;
                NotifyPropertyChanged("Subject");
            }
        }
        public DateTime BeginningDate
        {
            get { return _beginningDate; }
            set
            {
                _beginningDate = value;
                NotifyPropertyChanged("BeginningDate");
            }
        }
        public DateTime EndingDate
        {
            get { return _endingDate; }
            set
            {
                _endingDate = value;
                NotifyPropertyChanged("EndingDate");
            }
        }
        public int LocationId
        {
            get { return _locationId; }
            set
            {
                _locationId = value;
                NotifyPropertyChanged("LocationId");
            }
        }
        public string Room { get; set; }
        public ICollection<UserDTO> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                NotifyPropertyChanged("Users");
            }
        }

        public AppointmentModel()
        {
            
        }

        #region IDataErrorInfo
        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }
        #endregion

        #region Validation
        static readonly string[] ValidatedProperties =
        {
            "Subject", "Users", "LocationId", "BeginningDate"
        };

        public bool IsValid
        {
            get
            {
                foreach (var property in ValidatedProperties)
                {
                    if (GetValidationError(property) != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        string GetValidationError(string propertyName)
        {
            string error = null;

            switch (propertyName)
            {
                case "Subject":
                    error = ValidateSubject();
                    break;

                case "Users":
                    error = ValidateUsers();
                    break;

                case "LocationId":
                    error = ValidateLocationId();
                    break;

                case "BeginningDate":
                    error = ValidateBeginningDate();
                    break;
            }

            return error;
        }


        private string ValidateBeginningDate()
        {
            if (BeginningDate >= EndingDate)
            {
                return "Check beginning and ending date!";
            }
            return null;
        }
        private string ValidateLocationId()
        {
            if (LocationId == 0 || LocationId < 0)
            {
                return "Select location!";
            }
            return null;
        }
        private string ValidateUsers()
        {
            if (Users.Count == 0)
            {
                return "User list can not be empty!";
            }
            return null;
        }
        private string ValidateSubject()
        {
            if (String.IsNullOrWhiteSpace(Subject))
            {
                return "Subject can not be empty!";
            }
            if (Subject.Length > 50)
            {
                return "Subject must be less than 50 characters";
            }
            return null;
        }
        #endregion

        #region INotifyPropertyChanged
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
