namespace UTeM_Mobile.Core.Models
{
    public class PaginatedResponse<T> : Error
    {
        public Pagination<T> Data;
        public string Path;
        public string Convention;
        public bool IsSuccess;
    }
}
