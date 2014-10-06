using System;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using Google.GData.Documents;
using Google.GData.Client;
using Google.Contacts;

namespace GoogleServicesClient.Model
{
    /// <summary>
    /// Model class for Google Service ViewModels.
    /// </summary>
    public class ServicesModel
    {
        #region: Fields
        private static OAuth2Parameters _parameters = null;
        private static AutoResetEvent _autoEvent = new AutoResetEvent(true);
        private static Exception _exception = null;
        private static object _syncRoot = new object();
        //OAuth service parameter constants
        private const string CLIENT_ID = "1094227608591-o99lgq0tort7dodjqhl9gg06fl83l4oh.apps.googleusercontent.com";
        private const string CLIENT_SECRET = "HpY_c9876h0PRQkmiXjQN0_3";
        private const string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";
        private const string DRIVE_SCOPE = "https://docs.google.com/feeds/ ";
        private const string CONTACTS_SCOPE = "https://www.googleapis.com/auth/contacts.readonly";
        private const string STATE = "documents";
        private const string ACCESS_TYPE = "offline";
        private const string APPLICATION_NAME = "GoogleServicesClient";
        #endregion

        #region: Enums
        /// <summary>
        /// Indicates the service to be run.
        /// </summary>
        public enum ServiceType
        {
            Drive,
            Contacts
        }
        #endregion

        #region: Properties
        /// <summary>
        /// Holds error information for Model class.
        /// </summary>
        public static Exception Error
        {
            get
            {
                return _exception;
            }
            set
            {
                lock (_syncRoot)
                {
                    _exception = value;
                }
            }
        }
        #endregion

        #region: Methods
        /// <summary>
        /// Gets the Google service URL for fetching access code.
        /// </summary>
        /// <param name="viewName">The view name of the service to run.</param>
        /// <returns></returns>
        public static string GetServiceURL(string viewName)
        {
            //Initializing OAuthParameters for accessing the Google service(s).
            OAuth2Parameters parameters = new OAuth2Parameters()
            {
                ClientId = CLIENT_ID,
                ClientSecret = CLIENT_SECRET,
                RedirectUri = REDIRECT_URI,
                Scope = viewName == "Drive" ? DRIVE_SCOPE : CONTACTS_SCOPE,
                State = STATE,
                AccessType = ACCESS_TYPE
            };

            //Fetching URL for viewing the service's access code.
            string authUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(parameters);
            return authUrl;
        }

        /// <summary>
        /// Gains access to a Google service.
        /// </summary>
        /// <param name="type">The type of Google service to access.</param>
        /// <param name="accessCode">The access code entered by the user for the service.</param>
        private static void GetAccess(ServiceType type, string accessCode)
        {
            //Taking control of waithandle to prevent worker thread from fetching 
            //Drive/Contacts data asynchronously before the service's authentication takes place.
            _autoEvent.WaitOne();
            //Initializing Google service parameters (along with access code provided by user)
            OAuth2Parameters parameters = new OAuth2Parameters()
            {
                ClientId = CLIENT_ID,
                ClientSecret = CLIENT_SECRET,
                RedirectUri = REDIRECT_URI,
                Scope = type == ServiceType.Drive ? DRIVE_SCOPE : CONTACTS_SCOPE,
                State = STATE,
                AccessType = ACCESS_TYPE,
                AccessCode = accessCode.Trim()
            };
            _parameters = parameters;

            try
            {
                //Getting URL for viewing service Access Code.
                string authUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(_parameters);
                //Here we try to gain access to actual service.
                OAuthUtil.GetAccessToken(_parameters);
            }
            catch (Exception Ex)
            {
                _exception = Ex;
                return;
            }
            finally
            {
                //We can now signal other threads to go ahead with executing the actual service
                //since it is now authenticated.
                _autoEvent.Set();
            }
        }

        #region: Google Drive Service methods
        /// <summary>
        /// Provides access to Google drive service.
        /// </summary>
        /// <param name="accessCode">Access code of the service.</param>
        public static void GetGDriveAccess(string accessCode)
        {
            //Trying to gain access to Google Drive service. This is to keep the UI responsive during the 
            //service handshake.
            Thread thread = new Thread(() => GetAccess(ServiceType.Drive, accessCode));
            thread.Start();
        }
        /// <summary>
        /// Fetches list of documents in Google Drive.
        /// </summary>
        /// <returns></returns>
        public static DocumentsFeed GetGDriveDocList()
        {
            //Waiting for the service authentication to complete.
            _autoEvent.WaitOne();
            DocumentsFeed documents = null;
            try
            {
                //In case an error was encountered go back.
                if (_parameters == null || _exception != null)
                {
                    return documents;
                }
                //Creating the request factory for the Drive service.
                GOAuth2RequestFactory requestFactory = new GOAuth2RequestFactory(null, APPLICATION_NAME, _parameters);
                //Creating a service instance.
                DocumentsService service = new DocumentsService(APPLICATION_NAME);
                //Assigning the factory instance to the service.
                service.RequestFactory = requestFactory;
                DocumentsListQuery query = new DocumentsListQuery();
                //Sending a query to the service to fetch list of all documents on the drive.
                documents = service.Query(query);
            }
            catch (Exception Ex)
            {
                _exception = Ex;
            }
            finally
            {
                //Can now signal other threads to continue accessing/refreshing services.
                _autoEvent.Set();
            }
            return documents;
        }
        #endregion

        #region: Google Contacts Service methods
        /// <summary>
        /// Provides access to Google Contacts service.
        /// </summary>
        /// <param name="accessCode">Access code of the service.</param>
        public static void GetGContactsAccess(string accessCode)
        {
            //Trying to gain access to Google Contacts service. This is to keep the UI responsive during the 
            //service handshake.
            Thread thread = new Thread(() => GetAccess(ServiceType.Contacts, accessCode));
            thread.Start();
        }
        /// <summary>
        /// Fetches list of Google contacts.
        /// </summary>
        /// <returns></returns>
        public static Feed<Contact> GetGContactsList()
        {
            //Waiting for the service authentication to complete.
            _autoEvent.WaitOne();
            Feed<Contact> feed = null;
            try
            {
                //In case an error was encountered go back.
                if (_parameters == null || _exception != null)
                {
                    return feed;
                }
                //Creating the request factory for the Drive service.
                GOAuth2RequestFactory requestFactory = new GOAuth2RequestFactory(null, APPLICATION_NAME, _parameters);
                //Creating Request Settings instance
                RequestSettings settings = new RequestSettings(APPLICATION_NAME, _parameters);
                //Creating a request for fetching list of Google Contacts.
                ContactsRequest cr = new ContactsRequest(settings);
                //Fetching the list of Contacts.
                feed = cr.GetContacts();
            }
            catch (Exception Ex)
            {
                _exception = Ex;
            }
            finally
            {
                //Can now signal other threads to continue accessing/refreshing services.
                _autoEvent.Set();
            }
            return feed;
        }
        #endregion

        #endregion
    }
}
