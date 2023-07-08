using System.Net;

namespace UTeM_Mobile.Models
{
    public class ObjectResponse<T>
    {
        private T data;
        private string message;
        private bool isSuccess;
        private HttpStatusCode statusCode;

        public T Data { get => data; set => data = value; }
        public string Message { get => message; set => message = value; }
        public bool IsSuccess { get => isSuccess; set => isSuccess = value; }
        public HttpStatusCode StatusCode { get => statusCode; set => statusCode = value; }
    }
}
