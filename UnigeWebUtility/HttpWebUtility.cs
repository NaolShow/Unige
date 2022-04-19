namespace UnigeWebUtility
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    public static class HttpWebUtility
    {
        /// <summary>
        /// Sets the http web request content to the specified content type and the UTF8 encoded string
        /// </summary>
        public static void SetContent(this HttpWebRequest request, string contentType, string content)
            => SetContent(request, contentType, Encoding.UTF8.GetBytes(content));

        /// <summary>
        /// Sets the http web request content to the specified content type and byte array
        /// </summary>
        public static void SetContent(this HttpWebRequest request, string contentType, byte[] content)
        {
            // Set the request content type and length
            request.ContentType = contentType;
            request.ContentLength = content.Length;

            // Write the content to the request stream
            using var stream = request.GetRequestStream();
            stream.Write(content, 0, content.Length);
        }

        /// <summary>
        /// Gets the content of the http web response
        /// </summary>
        /// <returns></returns>
        public static string GetContent(this HttpWebResponse response)
        {
            // Get the response stream
            using var stream = response.GetResponseStream();

            // Get the encoding from the response
            var encoding = Encoding.GetEncoding(response.CharacterSet
                                                ?? throw new InvalidOperationException());

            // Convert the stream to a stream reader
            using var reader = new StreamReader(stream, encoding);

            return reader.ReadToEnd();
        }
    }
}