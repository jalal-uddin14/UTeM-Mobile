namespace UTeM_Mobile.Core.Models
{
    public class DBPatrol
    {
        public int Id { get; set; }
        public bool TimerEnabled { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string GuardId { get; set; }
        public int ShiftId { get; set; }
        public int RouteId { get; set; }
        public int TimeScheduleId { get; set; }
        public bool IsStarted { get; set; }
        public bool IsMissed { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsScheduled { get; set; }
    }
}
