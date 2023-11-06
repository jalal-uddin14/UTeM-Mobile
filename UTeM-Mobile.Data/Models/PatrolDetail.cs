namespace UTeM_Mobile.Data.Models
{
    public class PatrolDetail
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public string Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public virtual Patrol Patrol { get; set; }
    }
}
