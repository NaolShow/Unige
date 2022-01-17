using System.IO;
using System.Net;
using System.Text;

namespace UnigeWebUtility {

    public static class HttpWebUtility {

        /// <summary>
        /// Sets the http web request content to the specified content type and the UTF8 encoded string
        /// </summary>
        public static void SetContent(this HttpWebRequest request, string contentType, string content)
            => SetContent(request, contentType, Encoding.UTF8.GetBytes(content));

        /// <summary>
        /// Sets the http web request content to the specified content type and byte array
        /// </summary>
        public static void SetContent(this HttpWebRequest request, string contentType, byte[] content) {

            // Set the request content type and length
            request.ContentType = contentType;
            request.ContentLength = content.Length;

            // Write the content to the request stream
            using Stream stream = request.GetRequestStream();
            stream.Write(content, 0, content.Length);

        }

        /// <summary>
        /// Gets the content of the http web response
        /// </summary>
        public static string GetContent(this HttpWebResponse response) {

            // Get the response stream
            using Stream stream = response.GetResponseStream();

            // Convert the stream to a stream reader
            using StreamReader reader = new(stream);

            return reader.ReadToEnd();

        }

    }

}
