using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;

namespace WhatsAPI.UniversalApps.Libs.Utils.Common
{
    public class HttpRequest
    {
        /// <summary>
        /// Request to Server using GET
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static async Task<string> Get(string uri)
        {
            var httpClient = new HttpClient();
            var url = new Uri(uri);
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequestMessage.Headers.Add("User-Agent", Constants.Information.UserAgent);
            var response = await httpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsStringAsync();
            Logger.Log.Write(res, uri);
            return res;
        }
        // From : http://eren.ws/2013/10/20/http-multipartform-data-upload-in-windows-store-apps-boredom-challenge-day-16/
        public static byte[] BuildByteArray(string name, string fileName, byte[] fileBytes, string boundary)
        {
            // Create multipart/form-data headers.
            byte[] firstBytes = Encoding.UTF8.GetBytes(String.Format(
                "--{0}\r\n" +
                "Content-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\n" +
                "\r\n",
                boundary,
                name,
                fileName));

            byte[] lastBytes = Encoding.UTF8.GetBytes(String.Format(
                "\r\n" +
                "--{0}--\r\n",
                boundary));

            int contentLength = firstBytes.Length + fileBytes.Length + lastBytes.Length;
            byte[] contentBytes = new byte[contentLength];

            // Join the 3 arrays into 1.
            Array.Copy(
                firstBytes,
                0,
                contentBytes,
                0,
                firstBytes.Length);
            Array.Copy(
                fileBytes,
                0,
                contentBytes,
                firstBytes.Length,
                fileBytes.Length);
            Array.Copy(
                lastBytes,
                0,
                contentBytes,
                firstBytes.Length + fileBytes.Length,
                lastBytes.Length);

            return contentBytes;
        }
        public static async Task<HttpResponseMessage> UploadPhotoContinueAsync(byte[] files, string url, string boundary, Dictionary<string, string> headers = null, string fileName = "default.jpg")
        {

            HttpRequestMessage request = new HttpRequestMessage(
           System.Net.Http.HttpMethod.Post,
           new Uri(url));
            
            request.Content = new ByteArrayContent(
            files);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
            request.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue(
     "boundary",
     boundary));
            foreach (var header in headers)
            {
                request.Content.Headers.Add(header.Key, header.Value.ToString());
            }
            HttpClient client = new HttpClient();
            var attachmentResponse = await client.SendAsync(request);

            return attachmentResponse;
        }
    }
}
