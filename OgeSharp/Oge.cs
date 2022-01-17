using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using UnigeWebUtility;

namespace OgeSharp {

    public partial class Oge {

        #region Constants

        internal static readonly Uri LoginUri = new("https://casiut21.u-bourgogne.fr/login");
        internal static readonly Uri LogoutUri = new("https://casiut21.u-bourgogne.fr/logout");

        internal static readonly Uri HomeUri = new("https://iutdijon.u-bourgogne.fr/oge/stylesheets/etu/home.xhtml");

        internal static readonly Uri ScheduleUri = new("https://iutdijon.u-bourgogne.fr/oge/stylesheets/etu/planningEtu.xhtml");
        internal static readonly Uri GradesUri = new("https://iutdijon.u-bourgogne.fr/oge/stylesheets/etu/bilanEtu.xhtml");

        #endregion

        /// <summary>
        /// Fake web browser used by Oge instance
        /// </summary>
        public readonly FakeWebBrowser Browser = new();

        /// <summary>
        /// Latest username used for login into OGE (used for auto-reconnect)
        /// </summary>
        private string Username;
        /// <summary>
        /// Latest password used for login into OGE (used for auto-reconnect)
        /// </summary>
        private string Password;

        /// <summary>
        /// First name of the OGE user (only available after login)
        /// </summary>
        public string FirstName { get; private set; }
        /// <summary>
        /// Last name of the OGE user (only available after login)
        /// </summary>
        public string LastName { get; private set; }

        public Oge() {

            // Subscribe to the redirection event
            Browser.OnRedirection += OnRedirection;

        }

        // Triggered when the browser is getting redirected to another uri
        private void OnRedirection(object sender, RedirectionEventArgs e) {

            // If there is no username/password saved
            if (Username == null || Password == null) return;

            // If we are not getting redirected to the authentication page
            if (e.RedirectionUri.Host != LoginUri.Host || e.RedirectionUri.AbsolutePath != LoginUri.AbsolutePath) return;

            // Log out properly from OGE (prevents having some cookies alive on the authentication page but not on OGE)
            Browser.Navigate(LogoutUri);

            // Reconnect to OGE
            Login(Username, Password);

        }

        /// <summary>
        /// Logs in OGE using your username and password
        /// </summary>
        public bool Login(string username, string password) {

            // Download the login page source code and extract the execution token
            string loginSource = Browser.Navigate(LoginUri).GetContent();
            string execution = loginSource.Split("input type=\"hidden\" name=\"execution\" value=\"")[1].Split("\"/>")[0];

            // Create a POST request
            HttpWebRequest request = Browser.CreateRequest(LoginUri);
            request.Method = "POST";

            // Set the content (username, password, execution token and _eventId)
            request.SetContent("application/x-www-form-urlencoded", $"username={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}&execution={WebUtility.UrlEncode(execution)}&_eventId=submit");

            try {

                // Login by processing the request
                Browser.ProcessRequest(request);

                // Go to the home page to finish correctly the login process
                // => Fixes a bug where we get redirected to home after the first request
                string source = Browser.Navigate(HomeUri).GetContent();

                // Save the username and password
                // => After the login to prevent infinite logout/login loop
                Username = username;
                Password = password;

                #region First/Last name extraction

                // Parse the home page source code
                HtmlDocument document = new();
                document.LoadHtml(source);

                // Get the span's inner text (where there is the first and last name)
                string nameText = document.DocumentNode.SelectSingleNode("//div[@id='topFormMenu:j_id_x_content']/div[last()]/span[1]").InnerText;

                // Split it and assign it to the fields
                // => In case the last name is composed (like "Jean Albert Louis") we take only the first word as the First Name
                string[] splittedName = nameText.Split(' ');
                FirstName = splittedName[0];
                LastName = string.Join(' ', splittedName.Skip(1));

                #endregion

                return true;

            } catch (WebException ex) {

                // If the error is 401 unauthorized
                // => It means the user credentials are incorrect
                if (ex.Status == WebExceptionStatus.ProtocolError) return false;

                throw;

            }

        }

        /// <summary>
        /// Logs out of OGE
        /// </summary>
        public void Logout() {

            // Clear the username/password
            Username = Password = null;

            // Navigate to the logout uri
            Browser.Navigate(LogoutUri);

        }

    }

}
