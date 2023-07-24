using Newtonsoft.Json;
using System.Text;
using UTeM_Mobile.Core.IServices;
using UTeM_Mobile.Data.StaticCredentials;
using UTeM_Mobile.Models;

namespace UTeM_Mobile.Core.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {
        private static HttpClientHandler insecureHandler;
        private static HttpClient _client;
        private static string _baseURL = ServerCredential.BaseUrl;


        public async Task<ListResponse<T>> GetAllAsync(string url, AuthToken? token = null)
        {
            try
            {
                insecureHandler = GetInsecureHandler();
                _client = new HttpClient(insecureHandler);
                GetHttpClient(token);
                HttpResponseMessage response = await _client.GetAsync(ServerCredential.BaseUrl + url).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                ListResponse<T> listResponse = JsonConvert.DeserializeObject<ListResponse<T>>(result);
                listResponse.IsSuccess = response.IsSuccessStatusCode;
                return listResponse;
            }
            catch (Exception ex)
            {
                return new ListResponse<T>
                {
                    IsSuccess = false
                };
            }
        }


        public async Task<PaginatedResponse<T>> GetPagedListAsync(string url, AuthToken? token = null)
        {
            try
            {
                insecureHandler = GetInsecureHandler();
                _client = new HttpClient(insecureHandler);
                GetHttpClient(token);
                HttpResponseMessage response = await _client.GetAsync(_baseURL + url).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();
                PaginatedResponse<T> listResponse = JsonConvert.DeserializeObject<PaginatedResponse<T>>(result);
                listResponse.IsSuccess = response.IsSuccessStatusCode;
                return listResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ObjectResponse<T>> GetDetailsAsync(string url, AuthToken? token = null)
        {
            try
            {
                insecureHandler = GetInsecureHandler();
                _client = new HttpClient(insecureHandler);
                GetHttpClient(token);
                HttpResponseMessage HttpResponse = await _client.GetAsync(_baseURL + url).ConfigureAwait(false);
                string result = await HttpResponse.Content.ReadAsStringAsync();
                ObjectResponse<T>? objectResponse = JsonConvert.DeserializeObject<ObjectResponse<T>>(result);
                objectResponse.IsSuccess = HttpResponse.IsSuccessStatusCode;
                return objectResponse;
            }
            catch (Exception ex)
            {
                return new ObjectResponse<T>
                {
                    IsSuccess = false
                };
            }
        }

        public async Task<ObjectResponse<T>> InsertAsync(string url, object content, AuthToken? token = null)
        {
            try
            {
                insecureHandler = GetInsecureHandler();
                _client = new HttpClient(insecureHandler);
                GetHttpClient(token);
                var body = new StringContent(
                    JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                HttpResponseMessage HttpResponse = await _client.PostAsync(_baseURL + url, body).ConfigureAwait(false);
                string result = await HttpResponse.Content.ReadAsStringAsync();
                ObjectResponse<T> objectResponse = JsonConvert.DeserializeObject<ObjectResponse<T>>(result);
                objectResponse.IsSuccess = HttpResponse.IsSuccessStatusCode;
                objectResponse.StatusCode = HttpResponse.StatusCode;
                return objectResponse;
            }
            catch (Exception ex)
            {
                return new ObjectResponse<T>
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<ObjectResponse<T>> UpdateAsync(string url, object content, AuthToken? token = null)
        {
            try
            {
                insecureHandler = GetInsecureHandler();
                _client = new HttpClient(insecureHandler);
                GetHttpClient(token);
                var body = new StringContent(
                    JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                HttpResponseMessage HttpResponse = await _client.PutAsync(_baseURL + url, body).ConfigureAwait(false);
                string result = await HttpResponse.Content.ReadAsStringAsync();
                ObjectResponse<T> objectResponse = JsonConvert.DeserializeObject<ObjectResponse<T>>(result);
                objectResponse.IsSuccess = HttpResponse.IsSuccessStatusCode;
                objectResponse.StatusCode = HttpResponse.StatusCode;
                return objectResponse;
            }
            catch (Exception ex)
            {
                return new ObjectResponse<T>
                {
                    IsSuccess = false
                };
            }
        }
        public async Task<ObjectResponse<T>> DeleteAsync(string url, AuthToken token = null)
        {
            try
            {
                insecureHandler = GetInsecureHandler();
                _client = new HttpClient(insecureHandler);
                GetHttpClient(token);
                HttpResponseMessage HttpResponse = await _client.DeleteAsync(_baseURL + url).ConfigureAwait(false);
                string result = await HttpResponse.Content.ReadAsStringAsync();
                ObjectResponse<T> objectResponse = JsonConvert.DeserializeObject<ObjectResponse<T>>(result);
                objectResponse.IsSuccess = HttpResponse.IsSuccessStatusCode;
                return objectResponse;
            }
            catch (Exception ex)
            {
                return new ObjectResponse<T>
                {
                    IsSuccess = false
                };
            }
        }

        public async Task<ObjectResponse<T>> PostFile(string url, MultipartFormDataContent content, AuthToken token = null)
        {
            try
            {
                insecureHandler = GetInsecureHandler();
                _client = new HttpClient(insecureHandler);
                GetHttpClient(token);
                HttpResponseMessage HttpResponse = await _client.PostAsync(_baseURL + url, content).ConfigureAwait(false);
                string result = await HttpResponse.Content.ReadAsStringAsync();
                ObjectResponse<T> objectResponse = JsonConvert.DeserializeObject<ObjectResponse<T>>(result);
                objectResponse.IsSuccess = HttpResponse.IsSuccessStatusCode;
                objectResponse.StatusCode = HttpResponse.StatusCode;
                return objectResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static void GetHttpClient(AuthToken token)
        {
            if (_client == null)
            {
                _client = new HttpClient();
            }
            _client.DefaultRequestHeaders.Remove("Authorization");
            if (token != null)
            {
                _client.DefaultRequestHeaders.Add("Authorization", token.TokenType + " " + token.Token);
            }
        }

        // This method must be in a class in a platform project, even if
        // the HttpClient object is constructed in a shared project.
        public static HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
        }
    }
}
