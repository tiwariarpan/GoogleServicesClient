using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using GoogleServicesClient.Command;
using Google.GData.Client;

namespace GoogleServicesClient.ViewModel
{
    /// <summary>
    /// Base ViewModel class for ViewModels.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region: Fields
        protected OAuth2Parameters _parameters;
        protected string _submitBox = string.Empty;
        protected bool _canSubmit = true;
        protected bool _canRefresh = false;
        protected ObservableCollection<string> _data;
        DelegateCommand _submitCommand;
        DelegateCommand _reloadCommand;
        #endregion

        #region: INotifyPropertyChanged Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region: Properties
        /// <summary>
        /// Holds the Drive/Contacts data to be displayed on the view.
        /// </summary>
        public ObservableCollection<string> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Data"));
            }
        }
        /// <summary>
        /// Command for submiting access code received by the user.
        /// </summary>
        public ICommand SubmitCommand
        {
            get
            {
                if (_submitCommand == null)
                {
                    _submitCommand = new DelegateCommand(() => this.SubmitAndDisplay(), () => this.CanSubmit());
                }
                return _submitCommand;
            }
        }
        /// <summary>
        /// Command for refreshing data already visible in the view.
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                if (_reloadCommand == null)
                {
                    _reloadCommand = new DelegateCommand(() => this.RefreshAndDisplay(), () => this.CanRefresh());
                }
                return _reloadCommand;
            }
        }
        /// <summary>
        /// Holds text mapped to Access Code box.
        /// </summary>
        public string SubmitBox
        {
            get { return _submitBox; }
            set
            {
                _submitBox = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SubmitBox"));
            }
        }
        #endregion

        #region: Methods
        /// <summary>
        /// Checks if user is allowed to click on Submit.
        /// </summary>
        /// <returns></returns>
        public bool CanSubmit()
        {
            return _canSubmit;
        }
        /// <summary>
        /// Checks if user is allowed to click on Refresh.
        /// </summary>
        /// <returns></returns>
        public bool CanRefresh()
        {
            return _canRefresh;
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
        #endregion

        #region: Abstract methods
        /// <summary>
        /// Used for submitting user session's access code and displaying Drive/Contact data.
        /// </summary>
        protected abstract void SubmitAndDisplay();
        /// <summary>
        /// Used for refreshing data already visible in the view.
        /// </summary>
        protected abstract void RefreshAndDisplay();
        /// <summary>
        /// Loads Drive/Contact data in the view.
        /// </summary>
        protected abstract void LoadData();
        #endregion
    }
}
