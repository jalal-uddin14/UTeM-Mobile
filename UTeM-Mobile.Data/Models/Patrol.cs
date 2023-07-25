namespace UTeM_Mobile.Data.Models
{
    public class Patrol
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; }
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }
        public string GuardId { get; set; }
        public virtual ApplicationUser Guard { get; set; }
        public virtual IList<PatrolCheckpoint> PatrolCheckpoints { get; set; }
        public bool IsStarted { get { return Status == "Started"; } }
        public bool IsMissed { get { return Status == "Missed"; } }
        public bool IsCompleted { get { return Status == "Completed"; } }
        public bool IsScheduled { get { return Status == "Scheduled"; } }
    }
}
