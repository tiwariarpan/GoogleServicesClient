using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics;
using GoogleServicesClient.Command;
using GoogleServicesClient.Model;
using Google.GData.Client;

namespace GoogleServicesClient.ViewModel
{
    /// <summary>
    /// ViewModel for the Main Window.
    /// </summary>
    public class GoogleServicesClientViewModel : INotifyPropertyChanged
    {
        #region: Fields
        ViewModelBase _view;
        public event PropertyChangedEventHandler PropertyChanged;
        ICommand _viewServiceCommand;
        #endregion

        #region: Constructor
        public GoogleServicesClientViewModel()
        {
            _view = new DefaultViewModel();
        }
        #endregion

        #region: Properties
        /// <summary>
        /// Holds the current view being displayed on the Window.
        /// </summary>
        public ViewModelBase CurrentView
        {
            get { return _view; }
            set
            {
                _view = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentView"));
            }
        }

        /// <summary>
        /// Command to launch either of the services.
        /// </summary>
        public ICommand ViewServiceCommand
        {
            get
            {
                if (_viewServiceCommand == null)
                {
                    _viewServiceCommand = new DelegateCommand((o) => ViewService(o));
                }
                return _viewServiceCommand;
            }
        }
        #endregion

        #region: Methods
        /// <summary>
        /// Launches browser for viewing service access code and
        /// switches to the service's View.
        /// </summary>
        /// <param name="view">Name of the service View selected.</param>
        private void ViewService(object view)
        {
            string viewName = view as string;
            //Fetching service URL as per the service selection
            string authUrl = Model.ServicesModel.GetServiceURL(viewName);
            //Launch browser and direct user to the access code page.
            Process.Start(authUrl);
            //Swtich to corresponding service view.
            if (viewName == "Drive")
                CurrentView = new DriveViewModel();
            else
                CurrentView = new ContactsViewModel();
        }

        #region: INotifyPropertyChanged method
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
        #endregion

        #endregion

    }
}
