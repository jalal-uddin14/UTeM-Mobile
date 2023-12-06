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
        public int PatrolId { get; set; }
        public Patrol? Patrol { get; set; }
        public IList<PatrolCheckpoint> PatrolCheckpoints { get; set; }
        public bool IsStarted { get { return Status == "Started"; } }
        public bool IsMissed { get { return Status == "Missed"; } }
        public bool IsCompleted { get { return Status == "Completed"; } }
        public bool IsScheduled { get { return Status == "Scheduled"; } }
    }
}
