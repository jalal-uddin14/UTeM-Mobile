namespace UTeM_Mobile.Core.Models
{
    public class ListResponse<T>
    {
        private List<T> data;
        private string message;
        private bool isSuccess;

        public List<T> Data { get => data; set => data = value; }
        public string Message { get => message; set => message = value; }
        public bool IsSuccess { get => isSuccess; set => isSuccess = value; }
    }
}
