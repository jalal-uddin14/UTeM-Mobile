using Newtonsoft.Json;
using System.Text;
using UTeM_Mobile.Data.StaticCredentials;

namespace UTeM_Mobile.Core.Services
{
    public class APIService
    {
        static readonly HttpClientHandler insecureHandler = GetInsecureHandler();
        private static readonly HttpClient _client = new HttpClient(insecureHandler);
        private static readonly string _baseURL = ServerCredential.BaseUrl;

        public static async Task<HttpResponseMessage> GetAPI(string url)
        {
            return await _client.GetAsync(_baseURL + url).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> PostAPI(string url, object content)
        {
            var body = new StringContent(
                JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return await _client.PostAsync(_baseURL + url, body).ConfigureAwait(false);
        }

        public static async Task<HttpResponseMessage> PutAPI(string url, object content)
        {
            var body = new StringContent(
                JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            return await _client.PutAsync(_baseURL + url, body).ConfigureAwait(false);
        }


        // This method must be in a class in a platform project, even if
        // the HttpClient object is constructed in a shared project.
        public static HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (cert.Issuer.Equals("CN=localhost"))
                        return true;
                    return errors == System.Net.Security.SslPolicyErrors.None;
                }
            };
            return handler;
        }
    }
}
