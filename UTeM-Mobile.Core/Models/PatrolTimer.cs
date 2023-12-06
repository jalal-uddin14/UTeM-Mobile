namespace UTeM_Mobile.Core.Models
{
    public class PatrolTimer
    {
        public int PatrolId { get; set; }
        public int CheckpointId { get; set; }
        public DateTime? ExpectedCheckedTime { get; set; }
        public string CheckpointName { get; set; }
    }
}
