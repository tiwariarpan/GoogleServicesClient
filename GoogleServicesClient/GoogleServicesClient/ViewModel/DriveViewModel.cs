using System.Collections.ObjectModel;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using GoogleServicesClient.Model;
using GoogleServicesClient.Command;
using Google.GData.Client;
using Google.GData.Documents;

namespace GoogleServicesClient.ViewModel
{
    /// <summary>
    /// ViewModel for Drive View.
    /// </summary>
    public class DriveViewModel : ViewModelBase
    {
        #region: Constructor
        public DriveViewModel()
        {
            Data = new ObservableCollection<string>();
            _parameters = new OAuth2Parameters();
        }
        #endregion

        #region: Overriden Methods
        protected override void SubmitAndDisplay()
        {
            if (SubmitBox.Trim() == string.Empty)
            {
                MessageBox.Show("Please enter the access code from the browser window.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            //Gaining access to Google Drive service
            ServicesModel.GetGDriveAccess(SubmitBox);
            //Clearing Submit Box text
            SubmitBox = string.Empty;
            //Fetching document list on worker thread
            Thread thread = new Thread(() => LoadData());
            thread.Start();
            //Setting flags to disable submit and enable refresh.
            _canSubmit = false;
            _canRefresh = true;
        }

        protected override void RefreshAndDisplay()
        {
            //Simply reloading data and binding to UI
            Thread thread = new Thread(() => LoadData());
            thread.Start();
        }

        protected override void LoadData()
        {
            //Fetching user's Google drive document list.
            DocumentsFeed feed = ServicesModel.GetGDriveDocList();
            //Checking for errors in Model class
            if (ServicesModel.Error != null)
            {
                //Displaying error to user
                MessageBox.Show(ServicesModel.Error.Message + ".\n Please recheck the access code you've entered.", "Error accessing the service");
                //Clearing the error
                ServicesModel.Error = null;
                //Reenabling submit and disabling refresh
                _canSubmit = true;
                _canRefresh = false;
                return;
            }

            if (feed != null)
            {
                //Creating a temporary collection on the worker thread to avoid cross-thread exceptions.
                //This will keep the UI responsive while the thread iterates over the document list.
                ObservableCollection<string> temp = new ObservableCollection<string>();
                foreach (var entry in feed.Entries)
                {
                    temp.Add(entry.Title.Text);
                }
                //Assiging this collection to the Data collection bound to the View. 
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { Data = temp; }));
            }
        }
        #endregion
    }
}
