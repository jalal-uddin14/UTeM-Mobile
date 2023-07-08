namespace UTeM_Mobile.Models
{
    public class PaginatedResponse<T>
    {
        public Pagination<T> Data;
        public string Path;
        public string Convention;
        public bool IsSuccess;
    }
}
