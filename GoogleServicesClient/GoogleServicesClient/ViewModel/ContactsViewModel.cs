using System.Collections.ObjectModel;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using GoogleServicesClient.Command;
using GoogleServicesClient.Model;
using Google.GData.Client;
using Google.GData.Documents;
using Google.Contacts;

namespace GoogleServicesClient.ViewModel
{
    /// <summary>
    /// ViewModel for Contacts View.
    /// </summary>
    public class ContactsViewModel : ViewModelBase
    {
        #region: Constructor
        public ContactsViewModel()
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
            //Gaining Access to Google Contacts service.
            ServicesModel.GetGContactsAccess(SubmitBox);
            //Clearing the submit box
            SubmitBox = string.Empty;
            //Loading Contacts data on worker thread
            Thread thread = new Thread(() => LoadData());
            thread.Start();
            //Disabling sumbit and enabling refresh.
            _canSubmit = false;
            _canRefresh = true;
        }

        protected override void RefreshAndDisplay()
        {
            //Reloading Contacts' data on the UI.
            Thread thread = new Thread(() => LoadData());
            thread.Start();
        }

        protected override void LoadData()
        {
            //Fetching user's Contact list
            Feed<Contact> feed = ServicesModel.GetGContactsList();
            //Checking if Error encountered in Model
            if (ServicesModel.Error != null)
            {
                //Display the error to user
                MessageBox.Show(ServicesModel.Error.Message + ".\n Please recheck the access code you've entered.", "Error accessing the service");
                //Clearing the error
                ServicesModel.Error = null;
                //Reenable submit and disable refresh
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
                    if (entry.Title.Trim() != string.Empty)
                        temp.Add(entry.Title);
                }
                //Assiging this collection to the Data collection bound to the View. 
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { Data = temp; }));
            }

        }
        #endregion
    }
}
