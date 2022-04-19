namespace OgeSharp
{
    using System;
    using System.Linq;
    using System.Net;
    using HtmlAgilityPack;
    using UnigeWebUtility;

    public partial class Oge
    {
        internal static readonly Uri LoginUri = new("https://casiut21.u-bourgogne.fr/login");
        internal static readonly Uri LogoutUri = new("https://casiut21.u-bourgogne.fr/logout");

        internal static readonly Uri HomeUri = new("https://iutdijon.u-bourgogne.fr/oge/stylesheets/etu/home.xhtml");

        internal static readonly Uri ScheduleUri =
            new("https://iutdijon.u-bourgogne.fr/oge/stylesheets/etu/planningEtu.xhtml");

        internal static readonly Uri GradesUri =
            new("https://iutdijon.u-bourgogne.fr/oge/stylesheets/etu/bilanEtu.xhtml");


        /// <summary>
        /// Fake web browser used by Oge instance
        /// </summary>
        public readonly FakeWebBrowser Browser = new();

        /// <summary>
        /// Latest userName used for login into OGE (used for auto-reconnect)
        /// </summary>
        private string _username;

        /// <summary>
        /// Latest password used for login into OGE (used for auto-reconnect)
        /// </summary>
        private string _password;

        /// <summary>
        /// Gets first name of the OGE user (only available after login)
        /// </summary>
        public string FirstName { get; private set; }

        /// <summary>
        /// Gets last name of the OGE user (only available after login)
        /// </summary>
        public string LastName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Oge"/> class.
        /// </summary>
        public Oge()
        {
            // Subscribe to the redirection event
            Browser.OnRedirection += OnRedirection;
        }

        // Triggered when the browser is getting redirected to another uri
        private void OnRedirection(object sender, RedirectionEventArgs e)
        {
            // If there is no userName/password saved
            if (_username is null || _password is null)
            {
                return;
            }

            // If we are not getting redirected to the authentication page
            if (e.RedirectionUri.Host != LoginUri.Host ||
                e.RedirectionUri.AbsolutePath != LoginUri.AbsolutePath)
            {
                return;
            }

            // Log out properly from OGE (prevents having some cookies alive on the authentication page but not on OGE)
            Browser.Navigate(LogoutUri);

            // Reconnect to OGE
            Login(_username, _password);
        }

        /// <summary>
        /// Logs in OGE using your userName and password
        /// </summary>
        public bool Login(string userName, string password)
        {
            // Download the login page source code and extract the execution token
            var loginSource = Browser.Navigate(LoginUri).GetContent();
            var execution =
                loginSource.Split("input type=\"hidden\" name=\"execution\" value=\"")[1].Split("\"/>")[0];

            // Create a POST request
            var request = Browser.CreateRequest(LoginUri);
            request.Method = "POST";

            // Set the content (userName, password, execution token and _eventId)
            request.SetContent("application/x-www-form-urlencoded",
                $"userName={WebUtility.UrlEncode(userName)}&password={WebUtility.UrlEncode(password)}&execution={WebUtility.UrlEncode(execution)}&_eventId=submit");

            try
            {
                // Login by processing the request
                Browser.ProcessRequest(request);

                // Go to the home page to finish correctly the login process
                // => Fixes a bug where we get redirected to home after the first request
                var source = Browser.Navigate(HomeUri).GetContent();

                // Save the userName and password
                // => After the login to prevent infinite logout/login loop
                _username = userName;
                _password = password;

                // Parse the home page source code
                var document = new HtmlDocument();
                document.LoadHtml(source);

                // Get the span's inner text (where there is the first and last name)
                var nameText = document.DocumentNode
                    .SelectSingleNode("//div[@id='topFormMenu:j_id_x_content']/div[last()]/span[1]").InnerText;

                // Split it and assign it to the fields
                // => In case the last name is composed (like "Jean Albert Louis") we take only the first word as the First Name
                var splittedNames = nameText.Split(' ');
                FirstName = splittedNames[0];
                LastName = string.Join(' ', splittedNames.Skip(1));
                return true;
            }
            catch (WebException ex)
            {
                // If the error is 401 unauthorized
                // => It means the user credentials are incorrect
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    return false;
                }

                throw;
            }
        }

        /// <summary>
        /// Logs out of OGE
        /// </summary>
        public void Logout()
        {
            // Clear the userName/password
            _username = _password = null;

            // Navigate to the logout uri
            Browser.Navigate(LogoutUri);
        }
    }
}