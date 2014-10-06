using System.Collections.ObjectModel;
using Google.GData.Client;
using System.Diagnostics;
using Google.GData.Documents;
using Google.Contacts;
using System;
using GoogleServicesClient.Command;
using System.Windows.Input;
using System.Collections.Generic;

namespace GoogleServicesClient.ViewModel
{
    /// <summary>
    /// ViewModel for Default View.
    /// </summary>
    class DefaultViewModel : ViewModelBase
    {
        #region: Overriden Methods
        protected override void LoadData()
        {
            throw new NotImplementedException("LoadData not implemented on DefaultViewModel.");
        }

        protected override void SubmitAndDisplay()
        {
            throw new NotImplementedException();
        }

        protected override void RefreshAndDisplay()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
