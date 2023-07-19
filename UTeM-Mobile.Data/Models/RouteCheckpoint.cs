namespace UTeM_Mobile.Data.Models
{
    public class RouteCheckpoint
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public virtual Route Route { get; set; }
        public int CheckpointId { get; set; }
        public virtual Checkpoint Checkpoint { get; set; }

        public virtual bool IsNotLast { get; set; }
        public virtual bool IsChecked { get; set; }
        public virtual bool IsScheduled { get; set; }
        public virtual bool NotFound { get; set; } = true;
    }
}
