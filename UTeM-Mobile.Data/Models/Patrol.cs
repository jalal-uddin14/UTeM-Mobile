using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UTeM_Mobile.Data.Models
{
    public class Patrol
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
        public virtual ApplicationUser Guard { get; set; }
        public int ShiftId { get; set; }
        public virtual Shift Shift { get; set; }
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }
        public int TimeScheduleId { get; set; }
        public virtual TimeSchedule TimeSchedule { get; set; }

        public virtual IList<PatrolDetail> PatrolDetails { get; set; }
        public virtual IList<PatrolCheckpoint> PatrolCheckpoints { get; set; }
        public virtual bool IsStarted { get { return Status == "Started"; } }
        public virtual bool IsMissed { get { return Status == "Missed"; } }
        public virtual bool IsCompleted { get { return Status == "Completed"; } }
        public virtual bool IsScheduled { get { return Status == "Scheduled"; } }
    }
}
