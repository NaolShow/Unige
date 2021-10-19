using System;
using System.Net;

namespace Unige {

    /// <summary>
    /// A fake web browser that keeps it's cookies and tries to hide itself<br/>
    /// (This "browser" is not a browser. It uses <see cref="HttpWebRequest"/> so no javascript code is ran)
    /// </summary>
    public class FakeWebBrowser {

        /// <summary>
        /// Container that stores the cookies<br/>
        /// (You can modify them from here)
        /// </summary>
        public CookieContainer Cookies = new();

        /// <summary>
        /// Creates a http web request faking a web browser<br/>
        /// (Get the response of the request with <see cref="ProcessRequest(HttpWebRequest)"/>)
        /// </summary>
        public HttpWebRequest CreateRequest(Uri uri) {

            // Initialize a web request
            HttpWebRequest request = HttpWebRequest.CreateHttp(uri);

            // Quick fix for the wifi hotspot
            // TODO: Warn the user
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((a, b, c, d) => true);

            // Set the request user agent
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36";

            // Set the accept header
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";

            // Enable the automatic decompression
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

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
        public HttpWebResponse ProcessRequest(HttpWebRequest request) {

            // Process the request
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // If there is no cookies to save
            if (response.Headers.Count < (int)HttpResponseHeader.SetCookie) return response;

            // Save the cookies
            Cookies.SetCookies(response.ResponseUri, response.Headers[HttpResponseHeader.SetCookie]);

            return response;

        }

    }

}
