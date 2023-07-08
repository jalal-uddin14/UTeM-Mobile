namespace UTeM_Mobile.Models
{
    public class Pagination<T>
    {
        public List<T> PaginatedData { get; set; }
        public int Total { get; set; }
        public int Current_page { get; set; }
        public int Per_page { get; set; }
        public int Last_page { get; set; }
    }
}
