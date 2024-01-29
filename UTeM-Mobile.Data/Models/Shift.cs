namespace UTeM_Mobile.Data.Models
{
    public class Shift
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string? Status { get; set; }
    }
}
