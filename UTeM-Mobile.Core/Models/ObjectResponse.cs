using System.Net;

namespace UTeM_Mobile.Core.Models
{
    public class ObjectResponse<T> : Error
    {
        private T data;
        private bool isSuccess;
        private HttpStatusCode statusCode;

        public T Data { get => data; set => data = value; }
        public bool IsSuccess { get => isSuccess; set => isSuccess = value; }
        public HttpStatusCode StatusCode { get => statusCode; set => statusCode = value; }
    }
}
