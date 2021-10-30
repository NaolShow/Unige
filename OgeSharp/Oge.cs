using System;
using System.Net;
using UnigeWebUtility;

namespace OgeSharp {

    public partial class Oge {

        #region Constants

        internal static readonly Uri LoginUri = new Uri("https://casiut21.u-bourgogne.fr/login");
        internal static readonly Uri LogoutUri = new Uri("https://casiut21.u-bourgogne.fr/logout");

        internal static readonly Uri ScheduleUri = new Uri("https://iutdijon.u-bourgogne.fr/oge/stylesheets/etu/planningEtu.xhtml");
        internal static readonly Uri GradesUri = new Uri("https://iutdijon.u-bourgogne.fr/oge/stylesheets/etu/bilanEtu.xhtml");

        #endregion

        /// <summary>
        /// Fake web browser used by Oge instance
        /// </summary>
        public readonly FakeWebBrowser Browser = new FakeWebBrowser();

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
            request.SetContent("application/x-www-form-urlencoded", $"username={username}&password={password}&execution={execution}&_eventId=submit");

            try {

                // Login by processing the request
                Browser.ProcessRequest(request);
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
        public void Logout() => Browser.Navigate(LogoutUri);

    }

}
