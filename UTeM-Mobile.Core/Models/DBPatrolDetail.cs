using UTeM_Mobile.Data.Models;

namespace UTeM_Mobile.Core.Models
{
    public class DBPatrolDetail
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public string Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int PatrolId { get; set; }
        public bool IsStarted { get; set; }
        public bool IsMissed { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsScheduled { get; set; }
    }
}
