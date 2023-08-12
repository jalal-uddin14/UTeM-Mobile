namespace UTeM_Mobile.Data.Models
{
    public class PatrolCheckpoint
    {
        public int Id { get; set; }
        public DateTime? CheckedAt { get; set; }
        public DateTime? ExpectedCheckedTime { get; set; }
        public string Status { get; set; }
        public int PatrolId { get; set; }
        public virtual Patrol Patrol { get; set; }
        public int CheckpointId { get; set; }
        public virtual Checkpoint Checkpoint { get; set; }
        public virtual Report Report { get; set; }
    }
}
