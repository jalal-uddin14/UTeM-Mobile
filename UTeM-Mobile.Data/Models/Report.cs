namespace UTeM_Mobile.Data.Models
{
    public class Report
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string File { get; set; }
        public string GuardId { get; set; }
        public virtual ApplicationUser Guard { get; set; }
    }
}
