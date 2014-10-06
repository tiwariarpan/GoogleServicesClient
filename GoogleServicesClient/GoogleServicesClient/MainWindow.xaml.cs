using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GoogleServicesClient.ViewModel;

namespace GoogleServicesClient
{
    /// <summary>
    /// Interaction logic for MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region: Private fields
        private GoogleServicesClientViewModel _viewModel;
        #endregion

        #region: Constructor
        public MainWindow()
        {
            //Initializing default view model as GoogleServicesClientViewModel
            _viewModel = new GoogleServicesClientViewModel();
            InitializeComponent();
        }
        #endregion

        #region: Properties
        public GoogleServicesClientViewModel ViewModel
        {
            get { return _viewModel; }
            set { _viewModel = value; }
        }
        #endregion
    }
}
