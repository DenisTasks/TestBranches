using Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.EntitesDTO
{
    public class LocationDTO : IDataErrorInfo, INotifyPropertyChanged
    {
        public int LocationId { get; set; }
        public string Room { get; set; }

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
            "LocationId"
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
                case "LocationId":
                    error = ValidateLocationId();
                    break;
            }

            return error;
        }

        private string ValidateLocationId()
        {
            if (LocationId == 0 || LocationId < 0)
            {
                return "Select location!";
            }
            return null;
        }
        #endregion

        #region INotifyPropertyChanged
        [field: NonSerialized]
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
