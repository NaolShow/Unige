using System;
using System.Net;

namespace UnigeWebUtility
{
    /// <summary>
    /// A fake web browser that keeps it's cookies and tries to hide itself<br/>
    /// (This "browser" is not a browser. It uses <see cref="HttpWebRequest"/> so no javascript code is ran)
    /// </summary>
    public class FakeWebBrowser
    {
        /// <summary>
        /// Determines if the browser should allow redirections<br/>
        /// (You must change this here and not for each request at <see cref="HttpWebRequest.AllowAutoRedirect"/> else the cookies will not be saved)
        /// </summary>
        public bool AllowRedirections = true;

        /// <summary>
        /// Container that stores the cookies<br/>
        /// (You can modify them from here)
        /// </summary>
        public CookieContainer Cookies = new();

        /// <summary>
        /// Event that is triggered when the web browser is getting redirected to another uri
        /// </summary>
        public event EventHandler<RedirectionEventArgs> OnRedirection;

        /// <summary>
        /// Creates a http web request faking a web browser<br/>
        /// (Get the response of the request with <see cref="ProcessRequest(HttpWebRequest)"/>)
        /// </summary>
        /// <returns></returns>
        public HttpWebRequest CreateRequest(Uri uri)
        {
            // Initialize a web request
            var request = WebRequest.CreateHttp(uri);

            // Disable the auto redirection
            request.AllowAutoRedirect = false;

            // Quick fix for the wifi hotspot
            // TODO: Warn the user
            ServicePointManager.ServerCertificateValidationCallback =
                (a, b, c, d) => true;

            // Set the request user agent
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36";

            // Set the accept header
            request.Accept =
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";

            // Enable the automatic decompression
            request.AutomaticDecompression = DecompressionMethods.All;

            // Set the request cookies
            request.CookieContainer = Cookies;

            // Set the referer and the host
            request.Referer = uri.OriginalString;
            request.Host = uri.Host;

            return request;
        }

        /// <summary>
        /// Processes a http web request and saves the modified cookies
        /// </summary>
        /// <returns></returns>
        public HttpWebResponse ProcessRequest(HttpWebRequest request)
        {
            // Process the request
            var response = (HttpWebResponse) request.GetResponse();

            // If there is some cookies to save
            if (response.Headers[HttpResponseHeader.SetCookie] != null)
            {
                // Save the cookies
                Cookies.SetCookies(response.ResponseUri, response.Headers[HttpResponseHeader.SetCookie]);
            }

            // If there is a redirection and the browser allow redirections
            // => Navigate to the specified uri and return that response instead
            if (response.Headers[HttpResponseHeader.Location] != null && AllowRedirections)
            {
                // Create a uri from the location header
                Uri redirection = new(response.Headers[HttpResponseHeader.Location]);

                // Initialize new redirection event and trigger the redirection event
                OnRedirection?.Invoke(this, new RedirectionEventArgs()
                {
                    RequestedUri = request.RequestUri,
                    RedirectionUri = redirection,
                });

                return Navigate(redirection);
            }

            ;

            return response;
        }

        /// <summary>
        /// Navigates to the specified uri with the GET method
        /// </summary>
        /// <returns></returns>
        public HttpWebResponse Navigate(Uri uri) => ProcessRequest(CreateRequest(uri));
    }
}